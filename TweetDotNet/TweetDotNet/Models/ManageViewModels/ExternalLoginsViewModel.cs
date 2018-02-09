using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace TweetDotNet.Models.ManageViewModels
{
    public class ExternalLoginsViewModel
    {
        // List of multiple login accounts currently used by user
        public IList<UserLoginInfo> CurrentLogins { get; set; }

        // List of accounts Claims/Roles already used by logged in accounts
        public IList<AuthenticationScheme> OtherLogins { get; set; }

        // a flag property to allow user to remove an account already logged in
        public bool ShowRemoveButton { get; set; }

        // Message displayed to user showing their login status
        public string StatusMessage { get; set; }
    }
}
