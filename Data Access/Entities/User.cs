using Microsoft.AspNetCore.Identity;

namespace CollectionManager.Data_Access.Entities
{
    public class User: IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
    }
}
