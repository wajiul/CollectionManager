using CollectionManager.Data_Access.Entities;

namespace CollectionManager.Models
{
    public class CommentModel
    {
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int ItemId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
