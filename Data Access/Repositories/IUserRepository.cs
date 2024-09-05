using CollectionManager.Data_Access.Entities;
using CollectionManager.Models;

namespace CollectionManager.Data_Access.Repositories
{
    public interface IUserRepository
    {
        Task<UserProfileModel?> GetUserProfileAsync(string userId);
        Task<User?> GetUserAsync(string userId);
        Task UpdateSalesforceConnectionStatusAsync(string userId, bool status);
    }
}