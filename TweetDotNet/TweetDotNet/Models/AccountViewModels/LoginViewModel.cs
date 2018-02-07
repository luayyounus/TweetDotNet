using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TweetDotNet.Models.AccountViewModels
{
    public class LoginViewModel
    {
        // Email property requires the user with Email Address in the correct format 
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Password property requires the user with Password, Data type setup to be black dotted for secure typing 
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Flag for remembering user login
        // Display a message with data annotation when Invalid State model
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
