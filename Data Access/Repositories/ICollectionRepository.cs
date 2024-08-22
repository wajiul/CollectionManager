using CollectionManager.Data_Access.Entities;
using CollectionManager.Models;
using NpgsqlTypes;

namespace CollectionManager.Data_Access.Repositories
{
    public interface ICollectionRepository
    {
        Task CreateCollectionAsync(Collection collection);
        Task CreateCustomField(CustomField field);
        void DeleteCollection(Collection collection);
        void DeleteCustomField(CustomField customField);
        bool DoesUserHasCustomField(int Id, string userId);
        Task<Collection?> GetCollectionAsync(int Id);
        Task<List<string>> GetCollectionCategoriesAsync();
        Task<IEnumerable<CollectionWithItemCountModel>> GetCollections();
        Task<Collection?> GetCollectionWithCustomFieldAsync(int Id);
        Task<CollectionWithItemsReactionCountModel?> GetCollectionWithItemsReactionCount(int collectionId);
        Task<CustomField?> GetCustomFieldAsync(int Id);
        Task<List<CustomFieldValueModel>> GetCustomFieldsOfCollection(int collectionId);
        Task<IEnumerable<CollectionWithItemCountModel>> GetTopLargestCollectionsAsync();
        Task<IEnumerable<CollectionWithItemCountModel>> GetUserCollectionsAsync(string userId);
        bool IsCollectionExist(int Id);
        bool IsCollectionExist(int Id, string userId);
        bool IsCustomFieldExist(CustomField customField);
        void UpdateCollection(Collection collection);
        void UpdateSearchVector();
        Task<SearchResultModel> GetSearchResult(NpgsqlTsQuery fullTextQuery);

    }
}