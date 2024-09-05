using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CollectionManager.Models
{
    public class ContactModel
    {
        [Required]
        public string Country { get; set; } = string.Empty;
        [Required]
        public string City { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; } = string.Empty;
    }

    public class SalesforceAccountModel: ContactModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
