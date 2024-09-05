
using CollectionManager.Services;

namespace CollectionManager.Data_Access.Repositories
{
    public interface IUnitOfWork
    {
        ICollectionRepository Collection { get; }
        IItemRepository Item { get; }
        IUserRepository User { get; }
        IJiraService Jira { get; }
        Task Save();
    }
}