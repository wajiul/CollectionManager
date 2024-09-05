using CollectionManager.Enums;
using Microsoft.AspNetCore.Identity;

namespace CollectionManager.Data_Access.Entities
{
    public class User: IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserStatus Status { get; set; }
        public bool IsSalesforceConnected { get; set; }
        public ICollection<Collection> Collections { get; set; } = new List<Collection>();

    }
}
