namespace CollectionManager.Models
{
    public class MatchedItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CollectionId { get; set; }
    }

    public class MatchedCollectionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class SearchResultModel
    {
        public List<MatchedCollectionModel> Collections { get; set; }
        public List<MatchedItemModel> Items { get; set; }
    }
}
