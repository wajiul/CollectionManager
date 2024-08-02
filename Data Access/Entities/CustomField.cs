using Microsoft.VisualBasic.FileIO;

namespace CollectionManager.Data_Access.Entities
{
    public class CustomField
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public FieldType FieldType { get; set; }
        public int CollectionId { get; set; }
        public Collection Collection { get; set; }
    }
}
