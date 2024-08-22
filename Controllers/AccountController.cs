using CollectionManager.Data_Access;
using CollectionManager.Data_Access.Entities;
using CollectionManager.Enums;
using CollectionManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CollectionManager.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel loginData, string returnUrl =  "")
        {
            if(!ModelState.IsValid)
            {
                return View(loginData);
            }
            var user = await _userManager.FindByEmailAsync(loginData.Email);
            if(user != null && user.Status == UserStatus.Blocked) 
            {
                ModelState.AddModelError("", "User is blocked");
                return View(user);
            }
            var result = await _signInManager.PasswordSignInAsync(loginData.Email, loginData.Password, loginData.RememberMe, false);

            if(result.Succeeded) {

                TempData["ToastrMessage"] = "Successfully logged in";
                TempData["ToastrType"] = "success";

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
                return RedirectToAction("index", "home");
            }

            ModelState.AddModelError("", "wrong username / password");

            return View(loginData);

        }
        [HttpGet("signup")]
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpModel signUpData) 
        {
            if(!ModelState.IsValid) 
            {
                return View(signUpData);
            }
            var existingUser = await _userManager.FindByEmailAsync(signUpData.Email);
            if(existingUser != null) 
            {
                ModelState.AddModelError("", "Email already exist");
                return View(signUpData);
            }

            var newUser = new User 
            {
                FirstName = signUpData.FirstName,
                LastName = signUpData.LastName,
                Email = signUpData.Email,
                UserName = signUpData.Email,
                Status = UserStatus.Active
            };

            var result = await _userManager.CreateAsync(newUser, signUpData.Password);

            if(result.Succeeded) 
            {
                TempData["ToastrMessage"] = "Signed up successfully";
                TempData["ToastrType"] = "success";

                return RedirectToAction("Login");
            }

            foreach(var error in result.Errors) 
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(signUpData);
        }
        [Route("signout")]
        [Authorize]
        
        public async new Task<IActionResult> SignOut() {
            await _signInManager.SignOutAsync();
            TempData["ToastrMessage"] = "Successfully logged out";
            TempData["ToastrType"] = "success";
            return RedirectToAction("index", "home");
        }
    }
}
