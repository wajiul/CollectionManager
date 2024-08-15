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


            modelBuilder.Entity<Collection>()
                .HasIndex(c => new { c.UserId, c.Name, c.Category })
                .IsUnique();

            modelBuilder.Entity<CustomField>()
                .HasIndex(cf => new { cf.Name, cf.Type, cf.CollectionId })
                .IsUnique();

            modelBuilder.Entity<Item>()
            .Property(i => i.search_vector)
            .HasColumnType("tsvector");

            modelBuilder.Entity<Collection>()
            .Property(i => i.search_vector)
            .HasColumnType("tsvector");


            base.OnModelCreating(modelBuilder);
        }

        public void UpdateItemSearchVectors()
        {
            var sql = @"
            UPDATE public.""items""
            SET search_vector = to_tsvector('simple', ""Name"" || ' ' ||
                COALESCE((SELECT string_agg(concat(cfv.""Value"", ' ', cf.""Type""), ' ') 
                          FROM public.""customFieldValues"" AS cfv
                          JOIN public.""customFields"" AS cf ON cf.""Id"" = cfv.""CustomFieldId""
                          WHERE cfv.""ItemId"" = public.""items"".""Id""), '') || ' ' ||
                COALESCE((SELECT string_agg(""Text"", ' ') 
                          FROM public.""comments"" 
                          WHERE ""ItemId"" = public.""items"".""Id""), '')
            );";
            Database.ExecuteSqlRaw(sql);
        }

    }
}
