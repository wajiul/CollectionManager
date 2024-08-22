using CollectionManager.Models;
using CollectionManager.Data_Access;
using CollectionManager.Data_Access.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CollectionManager.Enums;
using Microsoft.AspNetCore.Authorization;
using CollectionManager.Data_Access.Repositories;

namespace CollectionManager.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/manage-users")]
    [Authorize(Roles = "admin")]
    public class ManageUsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        const string admin = "admin";
        public ManageUsersController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersWithRole = new List<UserWithSelectFlagModel>();
            foreach (var user in users)
            {
                usersWithRole.Add(new UserWithSelectFlagModel
                {
                    Id = user.Id,
                    Name = string.Concat(user.FirstName, ' ', user.LastName),
                    Email = user.Email,
                    Status = user.Status.ToString(),
                    IsAdmin = await _userManager.IsInRoleAsync(user, admin),
                    IsSelected = false
                });
            }

            return View(usersWithRole);
        }
        
        [HttpPost]
        public async Task<IActionResult> PerformAdminAction(List<UserWithSelectFlagModel> users, string operation)
        {
            var selectedUserIds = users.Where(u => u.IsSelected).Select(x => x.Id).ToList();

            if(selectedUserIds.Count == 0)
            {
                return RedirectToAction("Index");
            }

            foreach (var userId in selectedUserIds)
            {
                if(operation == "block")
                {
                    await Block(userId);
                }
                else if(operation == "unblock")
                {
                    await UnBlock(userId);
                }
                else if( operation == "delete")
                {
                    await Delete(userId);
                }
                else if(operation == "add_to_admin")
                {
                    await AddToAdmin(userId);
                }
                else if(operation == "remove_from_admin")
                {
                    await RemoveFromAdmin(userId);
                }
            }

            var currentAdmin = await _userManager.GetUserAsync(User);

            if(selectedUserIds.Contains(currentAdmin.Id) && (operation == "block" || operation == "delete" || operation == "remove_from_admin"))
            {
                if(operation == "block")
                {
                    await _signInManager.SignOutAsync();
                    RedirectToAction("Login", "Account");
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index");
        }
        
        private async Task Block(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return;
            }
            user.Status = UserStatus.Blocked;
            var result = await _userManager.UpdateAsync(user);
        }

        private async Task UnBlock(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return;
            }
            user.Status = UserStatus.Active;
            await _userManager.UpdateAsync(user);
        }

        private async Task Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return;
            }
            await _userManager.DeleteAsync(user);
        }

        private async Task AddToAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return;
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, admin);

            if(!isAdmin)
            {
                await _userManager.AddToRoleAsync(user, admin);
                await _userManager.UpdateSecurityStampAsync(user);
            }
        }

        private async Task RemoveFromAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return;
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, admin);

            if (isAdmin)
            {
                await _userManager.RemoveFromRoleAsync(user, admin);

            }
        }

    }
}
