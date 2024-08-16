using CollectionManager.Data_Access.Entities;
using Microsoft.Build.Framework;

namespace CollectionManager.Models
{
    public class CommentModel
    {
        [Required]
        public string Text { get; set; } = string.Empty;
        [Required]
        public string CreatedAt { get; set; } = string.Empty;
        [Required]
        public int ItemId { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        public string Commenter { get; set; } = string.Empty;
    }
}
