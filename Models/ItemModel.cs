using CollectionManager.Data_Access.Entities;

namespace CollectionManager.Models
{
    public class ItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        public int CollectionId { get; set; }
        public List<CustomFieldValueModel> FieldValues { get; set; } = new List<CustomFieldValueModel>();
    }

  

    public class ItemWithReactionModel : ItemViewModel
    {
        public int Likes { get; set; }
        public List<CommentModel> Comments { get; set; } = new List<CommentModel>();
    }

    public class ItemWithReactionCountModel : ItemViewModel
    {
        public int Likes { get; set; }
        public int Comments { get; set; }
    }

    public class ItemWithReactionCountAndDateModel : ItemWithReactionCountModel
    {
        public string CreatedAt { get; set; } = string.Empty;
    }

}
