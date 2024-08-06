namespace CollectionManager.Data_Access.Entities
{
    public class CustomFieldValue
    {
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public int CustomFieldId { get; set; }
        public CustomField CustomField { get; set; }

    }
}
