using CollectionManager.Data_Access.Entities;
using CollectionManager.Models;

namespace CollectionManager.Data_Access.Repositories
{
    public interface IItemRepository
    {
        Task AddCommentAsync(Comment comment);
        Task AddItemAsync(Item item);
        Task AddTagsAsync(Item item, List<Tag> tags);
        Task AddLikeAsync(Like like);
        Task Delete(int id);
        Task<CommentModel?> GetCommentAsync(int id);
        Task<Item?> GetItemAsync(int id);
        Task<IEnumerable<MatchedItemModel>> GetItemsByTagAsync(int tagId);
        Task<Tag?> GetTagAsync(string tagName);
        Task<ItemWithReactionModel> GetItemWithReactionsAsync(int itemId);
        Task<IEnumerable<ItemWithReactionCountAndDateModel>> GetRecentlyAddedItemAsync();
        Task<IEnumerable<TagModel>> GetTagsAsync();
        Task<int> GetTotalCommentOfItemAsync(int itemId);
        Task<int> GetTotalLikeOfItemAsync(int itemId);
        bool IsUserLikedAsync(int itemId, string userId);
        Task SaveAsync();
        void UpdateSearchVector();
    }
}