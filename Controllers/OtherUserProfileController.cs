using Microsoft.AspNetCore.Mvc;

namespace CollectionManager.Controllers
{
    [Route("profile/{userId}/collections")]
    public class OtherUserProfileController : Controller
    {
        [HttpGet("{collectionId}")]
        public IActionResult Index(int? collectionId, string? userId)
        {
            if(collectionId == null || userId == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = userId;
            return View(collectionId.Value);
        }

        [HttpGet("{collectionId}/items/{itemId}")]
        public IActionResult Items(int? itemId, string? userId)
        {
            if(itemId == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = userId;
            return View(itemId.Value);
        }
    }
}
