using CollectionManager.Services;

namespace CollectionManager.Data_Access.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CollectionMangerDbContext _context;
        public ICollectionRepository Collection { get; private set; }
        public IItemRepository Item { get; private set; }
        public IUserRepository User { get; private set; }   
        public IJiraService Jira { get; private set; }  
        public UnitOfWork(CollectionMangerDbContext context, IConfiguration configuration)
        {
            _context = context;
            Collection = new CollectionRepository(context);
            Item = new ItemRepository(context);
            User = new UserRepository(context);
            Jira = new JiraService(configuration);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
