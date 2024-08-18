using AutoMapper;
using CollectionManager.Data_Access.Entities;
using CollectionManager.Data_Access.Repositories;
using CollectionManager.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CollectionManager.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/user/{userId}/collections/{collectionId}/items")]
    public class ManageUserCollectionItemsController : Controller
    {
        private readonly CollectionRepository _collectionRepository;
        private readonly ItemRepository _itemRepository;
        private readonly IMapper _mapper;

        public ManageUserCollectionItemsController(CollectionRepository collectionRepository, ItemRepository itemRepository, IMapper mapper)
        {
            _collectionRepository = collectionRepository;
            _itemRepository = itemRepository;
            _mapper = mapper;
        }
        [HttpGet("")]
        public IActionResult Index(int collectionId, string userId)
        {
            var isExist = _collectionRepository.IsCollectionExist(collectionId, userId);
            if (!isExist)
            {
                return NotFound();
            }
            return View(collectionId);
        }

        [HttpGet("{id}")]
        public IActionResult Items(int? id, string userId)
        {
            ViewData["UserId"] = userId;
            return View(id);
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create(int collectionId)
        {
            var itemModel = new ItemModel();
            itemModel.CollectionId = collectionId;
            itemModel.FieldValues = await _collectionRepository.GetCustomFieldsOfCollection(collectionId);

            return View(itemModel);

        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(ItemModel newItem)
        {
            if (ModelState.IsValid)
            {
                var item = _mapper.Map<Item>(newItem);
                item.CreatedAt = DateTime.UtcNow;
                await _itemRepository.AddItemAsync(item);
                await _itemRepository.SaveAsync();
                return RedirectToAction("Index", "ManageUserCollectionItems", new { collectionId = item.CollectionId});
            }

            return View(newItem);
        }

        [HttpGet("{id}/edit")]
        public async Task<IActionResult> Edit(int Id)
        {
            var item = await _itemRepository.GetItemAsync(Id);
            var itemModel = _mapper.Map<ItemModel>(item);
            return View(itemModel);
        }

        [HttpPost("{id}/edit")]
        public async Task<IActionResult> Edit(int id, ItemModel updatedItem)
        {
            if (id != updatedItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var item = await _itemRepository.GetItemAsync(id);

                if (item == null)
                {
                    return NotFound();
                }

                item.Name = updatedItem.Name;

                var existingTags = item.Tags.ToList();
                var newTags = JsonConvert.DeserializeObject<List<TagValue>>(updatedItem.Tags);

                var tagsToAdd = newTags.Where(nt => existingTags.All(et => et.Name != nt.Value)).Select(nt => new Tag { Name = nt.Value }).ToList();

                var tagsToRemove = existingTags.Where(et => newTags.All(nt => nt.Value != et.Name))
                    .Select(et => new Tag
                    {
                        Name = et.Name
                    }
                ).ToList();

                foreach (var tag in tagsToAdd)
                {
                    item.Tags.Add(tag);
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

                await _itemRepository.SaveAsync();

                var userId = RouteData.Values["userId"].ToString();

                return RedirectToAction("Items", "ManageUserCollectionItems", new { id = item.Id, collectionId = item.CollectionId, userId = userId});
            }

            return View(updatedItem);
        }

        [HttpGet("{id}/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _itemRepository.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            var itemModel = _mapper.Map<ItemModel>(item);
            return View(itemModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _itemRepository.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            _itemRepository.Delete(item);
            await _itemRepository.SaveAsync();
            var userId = RouteData.Values["userId"].ToString();
            return RedirectToAction("Index", new {collectionId = item.CollectionId, userId = userId});
        }
    }
}
