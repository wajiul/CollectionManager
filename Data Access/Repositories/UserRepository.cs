using CollectionManager.Data_Access.Entities;
using CollectionManager.Models;
using Microsoft.EntityFrameworkCore;

namespace CollectionManager.Data_Access.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CollectionMangerDbContext _context;

        public UserRepository(CollectionMangerDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfileModel?> GetUserProfileAsync(string userId)
        {
            return await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserProfileModel
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    CollectionCount = u.Collections.Count,
                    IsSalesforceConnected = u.IsSalesforceConnected
                })
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserAsync(string userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task UpdateSalesforceConnectionStatusAsync(string userId, bool status)
        {
            var user = await _context.Users.FindAsync(userId);
            if(user != null)
            {
                user.IsSalesforceConnected = status;
            }
        }

    }
}
