using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetDotNet.Models
{
    public interface ITweetBlogService
    {
        List<TweetBlog> GetNewestFiveTweets(int number);
    }
}
