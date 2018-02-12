using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TweetDotNet.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        // Full name for user registeration with custom message
        [Required]
        [StringLength(50, ErrorMessage = "Max length is 50 characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        // Email property for user registeration with custom message using data annotation
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        // Password property requires the user with Password, Data type setup to be black dotted for secure typing 
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        // Confirmed Password property requires extra layer of safety if user entered a wrong character
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        // User Birthday when registered
        [DataType(DataType.Date)]
        [Display(Name = "Birthday")]
        public DateTime BirthDay { get; set; }

        // User's country
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }

        [Display(Name = "Years of Experience")]
        public int YearsOfExperience { get; set; }
    }
}
