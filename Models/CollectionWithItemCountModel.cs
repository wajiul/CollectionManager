namespace CollectionManager.Models
{
    public class CollectionWithItemCountModel: CollectionModel
    {
        public string Author { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }
}
