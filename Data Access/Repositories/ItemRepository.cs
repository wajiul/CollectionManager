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
        public async Task<IEnumerable<MatchedItemModel>> GetItemsByTagAsync(int tagId)
        {
            var tag = await _context.tags
                 .Where(t => t.Id == tagId)
                 .Include(t => t.Items)
                     .ThenInclude(c => c.Collection)
                 .Include(t => t.Items)
                     .ThenInclude(t => t.Likes)
                 .Include(t => t.Items)
                     .ThenInclude(t => t.Comments)
                 .FirstOrDefaultAsync();

           
            var itemModels = new List<MatchedItemModel>();
            foreach (var item in tag.Items)
            {
                var newItem = new MatchedItemModel
                {
                    Id= item.Id,
                    Name= item.Name,
                    CollectionId = item.CollectionId,
                    CollectionName = item.Collection.Name,
                    LikeCount = item.Likes.Count,
                    CommentCount = item.Comments.Count
                };
                itemModels.Add(newItem);
            }
            return itemModels;
        } 

        public async Task<int> GetTotalLikeOfItemAsync(int itemId)
        {
            return await _context.likes.CountAsync(x => x.ItemId == itemId);
        } 

        public async Task<int> GetTotalCommentOfItemAsync(int itemId)
        {
            return await _context.comments.CountAsync(x => x.ItemId == itemId);
        }
        public async Task AddItemAsync(Item item)
        {
            await _context.items.AddAsync(item);
        }
        public async Task<IEnumerable<ItemWithReactionCountAndDateModel>> GetRecentlyAddedItemAsync()
        {
            return await _context.items
                .OrderByDescending(i => i.CreatedAt)
                .Select(i => new ItemWithReactionCountAndDateModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    CreatedAt = i.CreatedAt.ToString("dd MMM, yyyy"),
                    CollectionId = i.CollectionId,
                    Likes = i.Likes.Count,
                    Comments = i.Comments.Count
                })
                .Take(10)
                .ToListAsync();
        }

        public async Task<IEnumerable<TagModel>> GetTagsAsync()
        {
            return await _context.tags
                .GroupBy(t => t.Name)
                .Select(g => new TagModel
                {
                    Id = g.FirstOrDefault().Id,
                    Name = g.Key
                })
                .ToListAsync();
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

        public async Task Delete(int id)
        {
            var item = await _context.items
                .Include(i => i.Tags)
                .Include(i => i.Comments)
                .Include(i => i.FieldValues)
                .FirstOrDefaultAsync(x => x.Id == id);  

            if (item != null)
            {
                _context.items.Remove(item);
            }
        }

        public void UpdateSearchVector()
        {
            _context.UpdateItemSearchVectors();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
       
    }
}