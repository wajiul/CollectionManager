using AutoMapper;
using CollectionManager.Data_Access;
using CollectionManager.Data_Access.Entities;
using CollectionManager.Data_Access.Repositories;
using CollectionManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace CollectionManager.Controllers
{
    [Route("/profile/my/collections/{collectionId}/items")]
    public class ProfileCollectionItemsController : Controller
    {
        private readonly CollectionMangerDbContext _context;
        private readonly CollectionRepository _collectionRepository;
        private readonly ItemRepository _itemRepository;
        private readonly IMapper _mapper;

        public ProfileCollectionItemsController(CollectionMangerDbContext context, CollectionRepository collectionRepository, ItemRepository itemRepository, IMapper mapper)
        {
            _context = context;
            _collectionRepository = collectionRepository;
            _itemRepository = itemRepository;
            _mapper = mapper;
        }

        [HttpGet("")]
        public IActionResult Index(int collectionId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var collectionExist = _collectionRepository.IsCollectionExist(collectionId, userId);

            if (!collectionExist)
            {
                return NotFound();
            }
            return View(collectionId);
        }

        [HttpGet("{id}")]
        public IActionResult Items(int? id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["UserId"] = userId;
            return View(id);
        }

        public async Task<IActionResult> Details(int Id)
        {
            var item = await _context.items
                .Include(t => t.Tags)
                .Include(f => f.FieldValues)
                    .ThenInclude(c => c.CustomField)
                .FirstOrDefaultAsync(x => x.Id == Id);

            var itemTags = new List<TagModel>();
            foreach (var itemTag in item.Tags)
            {
                itemTags.Add(new TagModel { Id = itemTag.Id, Name = itemTag.Name });
            }

            var fieldValues = new List<CustomFieldValueModel>();

            foreach (var field in item.FieldValues)
            {
                fieldValues.Add(new CustomFieldValueModel
                {
                    Id = field.Id,
                    Value = field.Value,
                    Name = field.CustomField.Name,
                    Type = field.CustomField.Type,
                    ItemId = item.Id
                });
            }

            var itemModel = new ItemModel
            {
                Name = item.Name,
                Tags = JsonConvert.SerializeObject(itemTags),
                FieldValues = fieldValues
            };

            return View(itemModel);
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

                await _context.items.AddAsync(item);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "ProfileCollectionItems", new { collectionId = item.CollectionId });
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
                    .Select(et => new Tag { 
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

                await _context.SaveChangesAsync();

                return RedirectToAction("Items", "ProfileCollectionItems", new { id = item.Id, collectionId = item.CollectionId});
            }

            return View(updatedItem);
        }

        [HttpGet("{id}/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _itemRepository.GetItemAsync(id);
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
            var item = await _itemRepository.GetItemAsync(id);
            if(item == null)
            {
                return NotFound();
            }
            _itemRepository.Delete(item);
            await _itemRepository.SaveAsync();
            return RedirectToAction("Index", new {collectionId = item.CollectionId});
        }


        [HttpPost("like")]
        public async Task<IActionResult> Like([FromBody] LikeModel like)
        {
            var liked = _itemRepository.IsUserLikedAsync(like.ItemId, like.UserId);
            if (liked)
            {
                return BadRequest(new { Message = "Already liked" });
            }

            var likeEntity = _mapper.Map<Like>(like);

            await _itemRepository.AddLikeAsync(likeEntity);
            await _itemRepository.SaveAsync();

            return Ok(new { Message = "Like added successfully" });
        }

        [HttpPost("comment")]
        public async Task<IActionResult> Comment([FromBody] CommentModel comment)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var commentEntity = _mapper.Map<Comment>(comment); 
            await _itemRepository.AddCommentAsync(commentEntity);
            await _itemRepository.SaveAsync();

            return Ok(new { Message = "Comment added successfully" });
        }

    }
}