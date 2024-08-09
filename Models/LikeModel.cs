using CollectionManager.Data_Access.Entities;
using System.ComponentModel.DataAnnotations;

namespace CollectionManager.Models
{
    public class LikeModel
    {
        [Required]
        public int ItemId { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;

    }
}
