using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetDotNet.Models;

namespace TweetDotNet.Components
{
    public class NewestFiveTweets : ViewComponent
    {
        private readonly ITweetBlogService _tweetBlogService;

        public NewestFiveTweets(ITweetBlogService service)
        {
            _tweetBlogService = service;
        }

        public IViewComponentResult Invoke(int x = 5)
        {
            var result = _tweetBlogService.GetNewestFiveTweets(x);
            return View(result);
        }
    }
}
