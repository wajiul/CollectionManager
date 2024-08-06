using CollectionManager.Data_Access;
using CollectionManager.Data_Access.Entities;
using CollectionManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CollectionManager.Controllers
{
    public class ItemsController : Controller
    {
        private readonly CollectionMangerDbContext _context;

        public ItemsController(CollectionMangerDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Show(int Id)
        {

            var collection = await _context.collections
                    .Include(c => c.Items)
                        .ThenInclude(i => i.Tags)
                    .Include(c => c.Items)
                        .ThenInclude(i => i.FieldValues)
                    .Include(c => c.CustomFields)
                    .FirstOrDefaultAsync(c => c.Id == Id);


            var collectionModel = new CollectionModel
            {
                Id = Id,
                Name = collection.Name,
                Description = collection.Description,
                ImageUrl = collection.ImageUrl,
                Category = collection.Category
            };

            foreach (var item in collection.Items)
            {
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

                var curItem = new ItemModel();
                curItem.Id = item.Id;
                curItem.Name = item.Name;
                curItem.CollectionId = item.CollectionId;
                curItem.FieldValues = fieldValues;

                var itemTags = new List<TagModel>();
                foreach (var itemTag in item.Tags)
                {
                    itemTags.Add(new TagModel { Id = itemTag.Id, Name = itemTag.Name });    
                }

                curItem.Tags = JsonConvert.SerializeObject(itemTags); 
                collectionModel.Items.Add(curItem);
            }

            return View(collectionModel);
        }

        public async Task<IActionResult> Create(int collectionId)
        {
            var fields = await _context.customFields
                .Where(x => x.CollectionId == collectionId).ToListAsync();

            var itemModel = new ItemModel();
            itemModel.CollectionId = collectionId;

            foreach(var cf in fields)
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

        [HttpPost]
        public async Task<IActionResult> Create(ItemModel newItem)
        {
            if(ModelState.IsValid)
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

                foreach(var field in newItem.FieldValues)
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

                return RedirectToAction("Index", new {Id = item.Id});
            }

           return View(newItem);
        }
    }
}
