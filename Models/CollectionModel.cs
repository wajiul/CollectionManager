using CollectionManager.Data_Access.Entities;
using System.ComponentModel.DataAnnotations;

namespace CollectionManager.Models
{
    public class CollectionModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string Category { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        [Required]
        public string UserId { get; set; } = string.Empty;
    }
    public class CollectionWithItemsModel: CollectionModel
    {
        public ICollection<ItemModel> Items { get; set; } = new List<ItemModel>();
    }

}
