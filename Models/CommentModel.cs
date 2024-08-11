using CollectionManager.Data_Access.Entities;
using Microsoft.Build.Framework;

namespace CollectionManager.Models
{
    public class CommentModel
    {
        [Required]
        public string Text { get; set; } = string.Empty;
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public int ItemId { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
    }
}
