using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollectionManager.Components
{
    [Route("profile")]
    public class ProfileController : Controller
    {

        [HttpGet("my")]
        public IActionResult MyProfile()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["UserId"] = userId;
            return View();
        }
        [HttpGet("{userId}")]
        public IActionResult Profile(string userId)
        {
            return View();
        }
    }
}
