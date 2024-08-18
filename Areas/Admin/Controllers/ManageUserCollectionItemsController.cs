using CollectionManager.Data_Access.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CollectionManager.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/user/{userId}/collections/{collectionId}/items")]
    public class ManageUserCollectionItemsController : Controller
    {
        private readonly CollectionRepository _collectionRepository;

        public ManageUserCollectionItemsController(CollectionRepository collectionRepository)
        {
            _collectionRepository = collectionRepository;
        }
        [HttpGet("")]
        public IActionResult Index(int collectionId, string userId)
        {
            var isExist = _collectionRepository.IsCollectionExist(collectionId, userId);
            if (!isExist)
            {
                return NotFound();
            }
            return View(collectionId);
        }

    }
}
