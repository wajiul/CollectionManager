namespace CollectionManager.Models
{
    public class MatchedItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CollectionId { get; set; }
        public string CollectionName { get; set; } = string.Empty;
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
    }

    public class MatchedCollectionModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }

    public class SearchResultModel
    {
        public List<MatchedCollectionModel> Collections { get; set; }
        public List<MatchedItemModel> Items { get; set; }
    }
}
