using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TweetDotNet.Models.ManageViewModels
{
    public class TwoFactorAuthenticationViewModel
    {
        // Defining if 2fa is used in a view model with a flag property
        public bool HasAuthenticator { get; set; }

        // Generated recovery codes necessary for 2fa
        public int RecoveryCodesLeft { get; set; }

        // checking flag if 2fa is enabled to be used in the project
        public bool Is2faEnabled { get; set; }
    }
}
