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

    public class CollectionWithCustomFieldModel : CollectionModel
    {
        public ICollection<CustomField> CustomFields { get; set; } = new List<CustomField>();
    }

    public class CollectionWithItemsReactionModel: CollectionModel
    {
        public IEnumerable<ItemWithReactionModel> Items { get; set; } = new List<ItemWithReactionModel>();
    }

    public class CollectionWithItemsReactionCountModel: CollectionModel
    {
        public IEnumerable<ItemWithReactionCountModel> Items { get; set; } = new List<ItemWithReactionCountModel>();
    }

}
