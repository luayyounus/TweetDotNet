using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TweetDotNet.Models.AccountViewModels
{
    // Forgot password view model used with account controller to retrieve forgotten password
    public class ForgotPasswordViewModel
    {
        // Email property required the user with Email Address in the correct format 
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
