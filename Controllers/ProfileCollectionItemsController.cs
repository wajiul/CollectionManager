using CollectionManager.Data_Access;
using CollectionManager.Data_Access.Entities;
using CollectionManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace CollectionManager.Controllers
{
    [Route("/profile/collections/{collectionId}/items")]
    public class ProfileCollectionItemsController : Controller
    {
        private readonly CollectionMangerDbContext _context;

        public ProfileCollectionItemsController(CollectionMangerDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index(int collectionId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var collectionExist = _context.collections.Any(c => c.UserId == userId && c.Id == collectionId);

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
            var fields = await _context.customFields
                .Where(x => x.CollectionId == collectionId).ToListAsync();

            var itemModel = new ItemModel();
            itemModel.CollectionId = collectionId;

            foreach (var cf in fields)
            {
                var customFieldValue = new CustomFieldValueModel()
                {
                    Id = cf.Id,
                    Name = cf.Name,
                    Type = cf.Type,
                };
                itemModel.FieldValues.Add(customFieldValue);
            }

            return View(itemModel);

        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(ItemModel newItem)
        {
            if (ModelState.IsValid)
            {
                var tagValues = JsonConvert.DeserializeObject<List<TagValue>>(newItem.Tags);

                var tagList = new List<Tag>();
                foreach (var tag in tagValues)
                {
                    tagList.Add(new Tag { Name = tag.Value });
                }

                var item = new Item
                {
                    Name = newItem.Name,
                    Tags = tagList,
                    CollectionId = newItem.CollectionId
                };

                foreach (var field in newItem.FieldValues)
                {
                    item.FieldValues.Add(new CustomFieldValue
                    {
                        Value = field.Value,
                        ItemId = field.ItemId,
                        CustomFieldId = field.Id
                    });
                }

                await _context.items.AddAsync(item);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "ProfileCollectionItems", new { collectionId = item.CollectionId });
            }

            return View(newItem);
        }

        [HttpGet("{id}/edit")]
        public async Task<IActionResult> Edit(int Id)
        {
            var item = await _context.items
                .Include(t => t.Tags)
                .Include(f => f.FieldValues)
                    .ThenInclude(c => c.CustomField)
                .FirstOrDefaultAsync(x => x.Id == Id);

            var itemTags = new List<string>();
            foreach (var itemTag in item.Tags)
            {
                itemTags.Add(itemTag.Name);
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
                Id = item.Id,
                Name = item.Name,
                Tags = JsonConvert.SerializeObject(itemTags),
                FieldValues = fieldValues
            };

            return View(itemModel);
        }

        [HttpPost("{id}/edit")]
        public async Task<IActionResult> Edit(int Id, ItemModel updatedItem)
        {
            if (Id != updatedItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var item = await _context.items
                    .Include(i => i.Tags)
                    .Include(i => i.FieldValues)
                    .FirstOrDefaultAsync(i => i.Id == Id);

                if (item == null)
                {
                    return NotFound();
                }

                item.Name = updatedItem.Name;

                var existingTags = item.Tags.ToList();
                var newTags = JsonConvert.DeserializeObject<List<TagValue>>(updatedItem.Tags);

                var tagsToAdd = newTags.Where(nt => existingTags.All(et => et.Name != nt.Value)).Select(nt => new Tag { Name = nt.Value }).ToList();
                var tagsToRemove = existingTags.Where(et => newTags.All(nt => nt.Value != et.Name)).Select(et => new Tag { Name = et.Name }).ToList();

                foreach (var tag in tagsToAdd)
                {
                    item.Tags.Add(tag);
                }

                foreach (var tag in tagsToRemove)
                {
                    item.Tags.Remove(tag);
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

        [HttpPost("like")]
        public async Task<IActionResult> Like([FromBody] LikeModel like)
        {
            var existing = await _context.likes.FirstOrDefaultAsync(l => l.UserId == like.UserId && l.ItemId == like.ItemId);
            if (existing != null)
            {
                return BadRequest(new { Message = "Already liked" });
            }
            var likeEntity = new Like
            {
                UserId = like.UserId,
                ItemId = like.ItemId
            };

            await _context.likes.AddAsync(likeEntity);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Like added successfully" });
        }

        [HttpPost("comment")]
        public async Task<IActionResult> Comment([FromBody] CommentModel comment)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            var commentEntity = new Comment
            {
                Text = comment.Text,
                UserId = comment.UserId,
                ItemId = comment.ItemId,
                CreatedAt = DateTime.Now
            };
            await _context.comments.AddAsync(commentEntity);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Comment added successfully" });
        }

    }
}