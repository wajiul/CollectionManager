using Microsoft.AspNetCore.Mvc;

namespace CollectionManager.Controllers
{
    [Route("profile/{userId}/collections")]
    public class OtherUserProfileController : Controller
    {
        [HttpGet("{id}")]
        public IActionResult Index(int? id, string? userId)
        {
            if(id == null || userId == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = userId;
            return View(id.Value);
        }

        [HttpGet("{collectionId}/items/{id}")]
        public IActionResult Items(int? id, string? userId)
        {
            if(id == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = userId;
            return View(id.Value);
        }
    }
}
