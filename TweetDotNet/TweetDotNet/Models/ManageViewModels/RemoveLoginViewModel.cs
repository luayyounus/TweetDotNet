using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TweetDotNet.Models.ManageViewModels
{
    public class RemoveLoginViewModel
    {
        // name of service provider logged in with by user
        public string LoginProvider { get; set; }

        // Token coming from the provider that allowed the user to be logged in
        public string ProviderKey { get; set; }
    }
}
