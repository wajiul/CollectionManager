using CollectionManager.Data_Access.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CollectionManager.Models;

namespace CollectionManager.Data_Access
{
    public class CollectionMangerDbContext : IdentityDbContext<User>
    {
        public CollectionMangerDbContext(DbContextOptions<CollectionMangerDbContext> options) : base(options)
        {

        }

        public DbSet<Collection> collections { get; set; }
        public DbSet<Item> items { get; set; }  
        public DbSet<Comment> comments { get; set; }
        public DbSet<Like> likes { get; set; }
        public DbSet<CustomField> customFields { get; set; }
        public DbSet<CustomFieldValue> customFieldValues { get; set; }   
        public DbSet<Tag> tags { get; set; }    


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Item)
                .WithMany(i => i.Comments)
                .HasForeignKey(c => c.ItemId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Like>()
                 .HasOne(i => i.Item)
                 .WithMany(l => l.Likes)
                 .HasForeignKey(f => f.ItemId)
                 .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CustomField>()
                .HasOne(c => c.Collection)
                .WithMany(d  => d.CustomFields)
                .HasForeignKey(f => f.CollectionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Collection>()
                .HasIndex(c => new {c.UserId, c.Name, c.Category })
                .IsUnique();

            modelBuilder.Entity<CustomField>()
                .HasIndex(cf => new { cf.Name, cf.Type, cf.CollectionId })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<CollectionManager.Models.ItemModel> ItemModel { get; set; } = default!;
    }
}
