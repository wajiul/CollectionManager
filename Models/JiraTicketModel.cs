using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CollectionManager.Models
{
    public class UserTicketInputModel
    {
        [Required]
        public string Summary { get; set; } = string.Empty;

        [Required]
        [DisplayName("Priority")]
        public string PriorityId { get; set; } = string.Empty;
    }

}
