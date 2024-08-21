using CollectionManager.Data_Access.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CollectionManager.Controllers
{
    [Route("profile")]
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        public ProfileController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("my")]
        public IActionResult MyProfile()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["UserId"] = userId;
            return View();
        }
       
    }
}
