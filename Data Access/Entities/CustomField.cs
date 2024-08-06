using CollectionManager.Enums;
namespace CollectionManager.Data_Access.Entities
{
    public class CustomField
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public CustomFieldType Type { get; set; }
        public int CollectionId { get; set; }
    }
}
