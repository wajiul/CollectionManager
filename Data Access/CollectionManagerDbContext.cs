using CollectionManager.Data_Access.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CollectionManager.Data_Access
{
    public class CollectionMangerDbContext : IdentityDbContext<User>
    {
        public CollectionMangerDbContext(DbContextOptions<CollectionMangerDbContext> options) : base(options)
        {

        }
    }
}
