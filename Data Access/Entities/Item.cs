using Azure;

namespace CollectionManager.Data_Access.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Tag> Tags { get; set; } = new List<Tag>();
        public int CollectionId { get; set; }
        public Collection Collection { get; set; }
        public List<CustomFieldValue> FieldValues { get; set; } = new List<CustomFieldValue>();

        public ICollection<Comment> Comments { get; set; }  
        public ICollection<Like> Likes { get; set; }

    }
}
