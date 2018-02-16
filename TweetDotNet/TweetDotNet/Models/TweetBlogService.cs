using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetDotNet.Data;

namespace TweetDotNet.Models
{
    public class TweetBlogService : ITweetBlogService
    {
        private readonly ApplicationDbContext _context;
        public TweetBlogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<TweetBlog> GetNewestFiveTweets(int number)
        {
            return _context.TweetBlogs.OrderByDescending(t => t.DateCreated).Take(number).ToList();
        }
    }
}
