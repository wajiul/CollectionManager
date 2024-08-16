using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;

namespace CollectionManager.Data_Access.Entities
{
    public class Collection
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; }
        public ICollection<Item> Items { get; set; } = new List<Item>();
        public ICollection<CustomField> CustomFields { get; set; } = new List<CustomField>();

        public NpgsqlTsVector? search_vector { get; set; }
    }
}
