﻿using CollectionManager.Data_Access;
using CollectionManager.Data_Access.Entities;
using CollectionManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
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
        public async Task<IActionResult> Index(int collectionId)
        {

            var collection = await _context.collections
                    .Include(c => c.Items)
                        .ThenInclude(i => i.Tags)
                    .Include(c => c.Items)
                        .ThenInclude(i => i.FieldValues)
                    .Include(c => c.CustomFields)
                    .FirstOrDefaultAsync(c => c.Id == collectionId);


            var collectionModel = new CollectionModel
            {
                Id = collectionId,
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

                return RedirectToAction("Show", new {collectionId = item.CollectionId});
            }

           return View(newItem);
        }


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

        [HttpPost]
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

                return RedirectToAction("Index", new { collectionId = item.CollectionId });
            }

            return View(updatedItem);
        }

    }
}
