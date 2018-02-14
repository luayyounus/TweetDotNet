using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetDotNet.Models
{
    public class TweetBlog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string HeadLine { get; set; }
        public DateTime DateCreated { get; set; }
        public string Content { get; set; }
    }
}
