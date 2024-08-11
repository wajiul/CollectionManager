using CollectionManager.Models;
using Microsoft.EntityFrameworkCore;

namespace CollectionManager.Data_Access.Repositories
{
    public class ItemRepository
    {
        private readonly CollectionMangerDbContext _context;

        public ItemRepository(CollectionMangerDbContext context)
        {
            _context = context;
        }

        public async Task<ItemWithReactionModel> GetItemWithReactionsAsync(int itemId)
        {
            var item = await _context.items
                    .Where(c => c.Id == itemId)
                    .Select(i => new ItemWithReactionModel
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
                            Name = f.CustomField.Name,
                            Type = f.CustomField.Type,
                            ItemId = f.CustomFieldId
                        }).ToList(),

                        Likes = i.Likes.Select(l => new LikeModel
                        {
                            ItemId = l.Id,
                            UserId = l.UserId,
                        }).ToList(),

                        Comments = i.Comments.Select(c => new CommentModel
                        {
                            ItemId = c.Id,
                            UserId = c.UserId,
                            Text = c.Text
                        }).ToList()

                    }).FirstOrDefaultAsync();


            return item;
        }
    }
}
