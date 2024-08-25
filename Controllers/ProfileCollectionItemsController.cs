using AutoMapper;
using CollectionManager.Data_Access;
using CollectionManager.Data_Access.Entities;
using CollectionManager.Data_Access.Repositories;
using CollectionManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace CollectionManager.Controllers
{
    [Route("/profile/my/collections/{collectionId}/items")]
    [Authorize]
    public class ProfileCollectionItemsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProfileCollectionItemsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("")]
        public IActionResult Index(int collectionId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var collectionExist = _unitOfWork.Collection.IsCollectionExist(collectionId, userId);

            if (!collectionExist)
            {
                return NotFound();
            }
            return View(collectionId);
        }

        [HttpGet("{itemId}")]
        public IActionResult Items(int? itemId, int? collectionId = null)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["UserId"] = userId;
            ViewData["CollectionId"] = collectionId;
            return View(itemId);
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create(int collectionId)
        {
            var itemModel = new ItemModel();
            itemModel.CollectionId = collectionId;
            itemModel.FieldValues = await _unitOfWork.Collection.GetCustomFieldsOfCollection(collectionId);

            return View(itemModel);

        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(ItemModel newItem)
        {
            if (ModelState.IsValid)
            {
                var item = _mapper.Map<Item>(newItem);
                item.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.Item.AddItemAsync(item);

                var tags = JsonConvert.DeserializeObject<List<TagValue>>(newItem.Tags).Select(t => new Tag
                {
                    Name = t.Value
                }).ToList();

                await _unitOfWork.Item.AddTagsAsync(item, tags);

                await _unitOfWork.Save();

                _unitOfWork.Item.UpdateSearchVector();

                TempData["ToastrMessage"] = "Item created successfully";
                TempData["ToastrType"] = "success";

                return RedirectToAction("Index", "ProfileCollectionItems", new { collectionId = item.CollectionId });
            }

            return View(newItem);
        }

        [HttpGet("{itemId}/edit")]
        public async Task<IActionResult> Edit(int itemId)
        {
            var item = await _unitOfWork.Item.GetItemAsync(itemId);
            var itemModel = _mapper.Map<ItemModel>(item);
            return View(itemModel);
        }

        [HttpPost("{itemId}/edit")]
        public async Task<IActionResult> Edit(int itemId, ItemModel updatedItem)
        {
            if (itemId != updatedItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var item = await _unitOfWork.Item.GetItemAsync(itemId);

                if (item == null)
                {
                    return NotFound();
                }

                item.Name = updatedItem.Name;

                var existingTags = item.Tags.ToList();
                var newTags = JsonConvert.DeserializeObject<List<TagValue>>(updatedItem.Tags);

                var tagsToAdd = newTags.Where(nt => existingTags.All(et => et.Name != nt.Value)).Select(nt => new Tag { Name = nt.Value }).ToList();
                
                var tagsToRemove = existingTags.Where(et => newTags.All(nt => nt.Value != et.Name))
                    .Select(et => new Tag { 
                        Name = et.Name
                    }
                ).ToList();

                foreach (var tag in tagsToAdd)
                {
                    var existingTag = await _unitOfWork.Item.GetTagAsync(tag.Name);
                    if (existingTag != null)
                    {
                        item.Tags.Add(existingTag);
                    }
                    else
                    {
                        item.Tags.Add(tag);
                    }
                }

                foreach (var tag in tagsToRemove)
                {
                    var tagRemove = item.Tags.FirstOrDefault(t => t.Name == tag.Name);
                    if (tagRemove != null)
                    {
                        item.Tags.Remove(tagRemove);
                    }
                }

                var existingFieldValues = item.FieldValues.ToList();
                var newFieldValues = updatedItem.FieldValues;

                foreach (var field in updatedItem.FieldValues)
                {
                    var fieldValue = existingFieldValues.FirstOrDefault(x => x.Id == field.Id);
                    if (fieldValue != null)
                    {
                        fieldValue.Value = field.Value;
                    }
                }

                await _unitOfWork.Save();
                _unitOfWork.Item.UpdateSearchVector();

                TempData["ToastrMessage"] = "Item updated successfully";
                TempData["ToastrType"] = "success";

                return RedirectToAction("Items", "ProfileCollectionItems", new { itemId = item.Id, collectionId = item.CollectionId});
            }

            return View(updatedItem);
        }

        [HttpGet("{itemId}/delete")]
        public async Task<IActionResult> Delete(int itemId)
        {
            var item = await _unitOfWork.Item.GetItemAsync(itemId);
            if(item == null)
            {
                return NotFound();
            }
            var itemModel = _mapper.Map<ItemModel>(item);
            return View(itemModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _unitOfWork.Item.GetItemAsync(id);
            if(item == null)
            {
                return NotFound();
            }
            await _unitOfWork.Item.Delete(id);
            await _unitOfWork.Save();
            _unitOfWork.Item.UpdateSearchVector();

            TempData["ToastrMessage"] = "Item deleted successfully";
            TempData["ToastrType"] = "success";

            return RedirectToAction("Index", new {collectionId = item.CollectionId});
        }

        [HttpGet]
        [Route("/tags")]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _unitOfWork.Item.GetTagsAsync();
            var tagStringList = tags.Select(x => x.Name).Distinct().ToList();
            return Ok(tagStringList);
        }

    }
}