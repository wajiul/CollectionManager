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
    }
}
