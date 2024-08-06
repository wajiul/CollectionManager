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
        public async Task<IActionResult> Index(int Id)
        {
            var item = await _context.items.Include(c => c.FieldValues).FirstOrDefaultAsync(x => x.Id == Id);
            return Ok(item);
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
                //var jsonTags = newItem.Tags.Split(',').ToList();
                var tagValues = JsonConvert.DeserializeObject<List<TagValue>>(newItem.Tags);
                //var tags = JsonConvert.DeserializeObject<List<string>>(newItem.Tags);

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
                try
                {
                    await _context.items.AddAsync(item);
                    await _context.SaveChangesAsync();
                }
                catch(Exception ex)
                {

                }

                return RedirectToAction("Index", new {Id = item.Id});
            }

           return View(newItem);
        }
    }
}
