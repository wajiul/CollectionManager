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
}
