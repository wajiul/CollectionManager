using CollectionManager.Data_Access.Entities;
using Microsoft.EntityFrameworkCore;

namespace CollectionManager.Data_Access.Repositories
{
    public class CollectionRepository
    {
        private readonly CollectionMangerDbContext _context;

        public CollectionRepository(CollectionMangerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Collection>> GetCollections()
        {
           return await _context.collections.ToListAsync();
        } 
    }
}
