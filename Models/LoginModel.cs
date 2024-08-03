﻿using System.ComponentModel.DataAnnotations;

namespace CollectionManager.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;    
        public bool RememberMe {get; set;}
    }
}
