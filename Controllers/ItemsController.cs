using CollectionManager.Data_Access;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollectionManager.Controllers
{
    public class ItemsController : Controller
    {
        private readonly CollectionMangerDbContext _context;

        public ItemsController(CollectionMangerDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int collectionId)
        {
            var collection = await _context.collections
                .Include(c => c.Items)
                .FirstOrDefaultAsync(x => x.Id == collectionId);

            return Ok(collection);
        }

        
    }
}
