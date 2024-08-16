using CollectionManager.Data_Access.Entities;
using CollectionManager.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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

                        Likes = i.Likes.Count,

                        Comments = i.Comments.OrderByDescending(c => c.CreatedAt).Select(c => new CommentModel
                        {
                            ItemId = c.Id,
                            UserId = c.UserId,
                            Text = c.Text,
                            CreatedAt = c.CreatedAt.ToString("MMMM yyyy"),
                            Commenter = string.Concat(c.User.FirstName, ' ', c.User.LastName)
                        }).ToList()

                    }).FirstOrDefaultAsync();


            return item;
        }

        public async Task<Item?> GetItemAsync(int id)
        {
            return await _context.items
                .Include(t => t.Tags)
                .Include(f => f.FieldValues)
                    .ThenInclude(c => c.CustomField)
                .FirstOrDefaultAsync(x => x.Id == id);  
        }

        public bool IsUserLikedAsync(int itemId, string userId)
        {
            return  _context.likes.Any(x => x.ItemId == itemId && x.UserId == userId);
        } 

        public async Task AddLikeAsync(Like like)
        {
           await _context.likes.AddAsync(like);
        }

        public async Task<CommentModel?> GetCommentAsync(int id)
        {
            return await _context.comments
                .Where(c => c.Id == id)
                .Select(c => new CommentModel
                {
                    ItemId= c.Id,
                    UserId= c.UserId,
                    Text = c.Text,
                    CreatedAt = c.CreatedAt.ToString("MMMM yyyy"),
                    Commenter = string.Concat(c.User.FirstName, ' ', c.User.LastName)
                }).FirstOrDefaultAsync();
        }


        public async Task AddCommentAsync(Comment comment)
        {
            await _context.comments.AddAsync(comment);
        }

        public void Delete(Item item)
        {
            _context.items.Remove(item);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
       
    }
}