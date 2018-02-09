using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TweetDotNet.Models.ManageViewModels
{
    public class IndexViewModel
    {
        // Main Index view model user name
        public string Username { get; set; }

        // main view model flag checking if email is confirmed to be valid and addresses the right user
        public bool IsEmailConfirmed { get; set; }

        // Email property for user registeration with custom message using data annotation
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Phone number property specifiying the number of the user logged/registered
        [Phone] // data annotation represetning the phone format
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public string StatusMessage { get; set; }
    }
}
