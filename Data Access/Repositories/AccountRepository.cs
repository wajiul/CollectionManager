using CollectionManager.Models;

namespace CollectionManager.Data_Access.Repositories
{
    public class AccountRepository
    {
        private readonly CollectionMangerDbContext _context;

        public AccountRepository(CollectionMangerDbContext context)
        {
            _context = context;
        }

        
    }
}
