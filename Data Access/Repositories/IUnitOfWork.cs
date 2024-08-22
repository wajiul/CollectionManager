
namespace CollectionManager.Data_Access.Repositories
{
    public interface IUnitOfWork
    {
        ICollectionRepository Collection { get; }
        IItemRepository Item { get; }

        Task Save();
    }
}