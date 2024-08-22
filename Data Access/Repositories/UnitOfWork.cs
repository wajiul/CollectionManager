namespace CollectionManager.Data_Access.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CollectionMangerDbContext _context;
        public ICollectionRepository Collection { get; private set; }
        public IItemRepository Item { get; private set; }

        public UnitOfWork(CollectionMangerDbContext context)
        {
            _context = context;
            Collection = new CollectionRepository(context);
            Item = new ItemRepository(context);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
