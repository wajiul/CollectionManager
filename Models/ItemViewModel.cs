using CollectionManager.Data_Access.Entities;

namespace CollectionManager.Models
{
    public class ItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Tag> Tags { get; set; }
        public int CollectionId { get; set; }
        public List<CustomFieldValueModel> FieldValues { get; set; } = new List<CustomFieldValueModel>();
    }
}
