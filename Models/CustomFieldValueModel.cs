using CollectionManager.Data_Access.Entities;
using CollectionManager.Enums;

namespace CollectionManager.Models
{
    public class CustomFieldValueModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public CustomFieldType Type { get; set; }
        public string Value { get; set; } = string.Empty;
        public int ItemId { get; set; }
    }
}
