using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TweetDotNet.Models.AccountViewModels
{
    // External view model used with Account Model when login in from an external resource
    public class ExternalLoginViewModel
    {
        // Email property required the user with Email Address in the correct format 
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
