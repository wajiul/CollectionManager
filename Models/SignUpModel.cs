using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CollectionManager.Models
{
    public class SignUpModel
    {
        [Required]
        [DisplayName("First name")]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [DisplayName("Last name")]
        public string LastName { get; set; }= string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        [DisplayName("Confirm password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
