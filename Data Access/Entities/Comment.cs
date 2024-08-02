namespace CollectionManager.Data_Access.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int ItemId { get; set; }
        public Item Item { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }

}
