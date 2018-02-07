using System;

namespace TweetDotNet.Models
{
    // Error view model returning login/register request id if exists
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}