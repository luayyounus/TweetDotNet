using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TweetDotNet.Models.ManageViewModels
{
    public class EnableAuthenticatorViewModel
    {
        // Email property for user registeration with custom and error messages using data annotation
        // its reuried and limited to only 7 characters
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; }

        // Shared key a previously shared key between parties in secure channeling
        // This property is set to never be binded when view model is used in Login/Register
        [BindNever]
        public string SharedKey { get; set; }

        // Matrix barcode not to be binded when used in a view model for safety
        [BindNever]
        public string AuthenticatorUri { get; set; }
    }
}
