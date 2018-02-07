using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TweetDotNet.Models.AccountViewModels
{
    public class LoginWith2faViewModel
    {
        // Required two factoring authentication generated when requested login
        // Display a error message with data annotation when Invalid State model
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Authenticator code")]
        public string TwoFactorCode { get; set; } // usually 6 numeric digits

        // Custom message with data annotation when Invalid State model
        // Flag to remember the device a user logged from
        [Display(Name = "Remember this machine")]
        public bool RememberMachine { get; set; }

        // Optional remember me flag to remember the user when authenticated
        public bool RememberMe { get; set; }
    }
}
