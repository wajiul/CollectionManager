using CollectionManager.Data_Access.Entities;
using CollectionManager.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CollectionManager.Data_Access.Repositories
{
    public class CollectionRepository
    {
        private readonly CollectionMangerDbContext _context;

        public CollectionRepository(CollectionMangerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CollectionWithItemCountModel>> GetCollections()
        {
            return await _context.collections
                 .Select(c => new CollectionWithItemCountModel
                 {
                     Id = c.Id,
                     Name = c.Name,
                     Description = c.Description,
                     Category = c.Category,
                     ImageUrl = c.ImageUrl,
                     Author = c.User.FirstName,
                     UserId = c.UserId,
                     ItemCount = c.Items.Count,
                 })
                 .ToListAsync();
        } 

        
        public async Task<IEnumerable<CollectionWithItemCountModel>> GetUserCollectionsAsync(string userId)
        {
            return await _context.collections
                 .Where(c => c.UserId == userId)
                 .Select(c => new CollectionWithItemCountModel
                 {
                     Id = c.Id,
                     Name = c.Name,
                     Description = c.Description,
                     Category = c.Category,
                     ImageUrl = c.ImageUrl,
                     Author = c.User.FirstName,
                     UserId = c.UserId,
                     ItemCount = c.Items.Count,
                 })
                 .ToListAsync();
        }
        public async Task<CollectionWithItemsReactionCountModel?> GetCollectionWithItemsReactionCount(int collectionId)
        {
            var collection = await _context.collections
                    .Where(c => c.Id == collectionId)
                    .Select(c => new CollectionWithItemsReactionCountModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        Category = c.Category,
                        ImageUrl = c.ImageUrl,
                        UserId = c.UserId,
                        Items = c.Items.Select(i => new ItemWithReactionCountModel
                        {
                            Id = i.Id,
                            Name = i.Name,
                            Tags = i.Tags.Select(t => new TagModel
                            {
                                Id = t.Id,
                                Name = t.Name,
                            }).ToList(),

                            FieldValues = i.FieldValues.Select(f => new CustomFieldValueModel
                            {
                                Id = f.Id,
                                Value = f.Value,
                                Type = f.CustomField.Type,
                                ItemId = f.CustomFieldId
                            }).ToList(),
                            Likes = i.Likes.Count,
                            Comments = i.Comments.Count
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

            return collection;
        }

        public async Task<List<CustomFieldValueModel>> GetCustomFieldsOfCollection(int collectionId)
        {
            return await _context.customFields
                .Where(c => c.CollectionId == collectionId)
                .Select(c => new CustomFieldValueModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type
                }).ToListAsync();
        }

        public async Task<Collection?> GetCollectionAsync(int Id)
        {
            return await _context.collections.FindAsync(Id);
        }

        public async Task<List<string>> GetCollectionCategoriesAsync()
        {
            return await _context.collections.Select(c => c.Category).Distinct().ToListAsync();
        }

        public async Task<CustomField?> GetCustomFieldAsync(int Id)
        {
            return await _context.customFields.FindAsync(Id);
        }
        public async Task<Collection?> GetCollectionWithCustomFieldAsync(int Id)
        {
            return await _context.collections
                .Include(c => c.CustomFields)
                .FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<IEnumerable<CollectionWithAuthorModel>> GetTopLargestCollectionsAsync()
        {
            return await _context.collections
                .OrderByDescending(c => c.Items.Count)
                .Select(c => new CollectionWithAuthorModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    UserId = c.UserId,
                    Author = string.Concat(c.User.FirstName, ' ', c.User.LastName)
                })
                .Take(5)
                .ToListAsync();
        }

        public bool IsCollectionExist(int Id)
        {
            return _context.collections.Any(c => c.Id == Id);
        }

        public bool IsCollectionExist(int Id, string userId)
        {
            return _context.collections.Any(c => c.UserId == userId && c.Id == Id);
        }
        public bool IsCustomFieldExist(CustomField customField)
        {
            return _context.customFields
                .Any(
                    c => c.CollectionId == customField.CollectionId &&
                    c.Name == customField.Name &&
                    c.Type == customField.Type);
        }

        public bool DoesUserHasCustomField(int Id, string userId)
        {
            return _context.collections
                .Any(c => c.UserId == userId && c.CustomFields.Any(cf => cf.Id == Id));
        }

        public async Task CreateCollectionAsync(Collection collection)
        {
            await _context.collections.AddAsync(collection);
        }

        public async Task CreateCustomField(CustomField field)
        {
            await _context.customFields.AddAsync(field);
        }
        public void UpdateCollection(Collection collection)
        {
            _context.collections.Update(collection);
        }
        public void DeleteCollection(Collection collection)
        {
            _context.collections.Remove(collection);
        }
        public void DeleteCustomField(CustomField customField)
        {
            _context.customFields .Remove(customField);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }


    }
}
