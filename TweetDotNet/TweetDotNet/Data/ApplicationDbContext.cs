using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TweetDotNet.Models;

namespace TweetDotNet.Data
{
    // Main App dbcontext inheriting from Identity Db context with Roles and GUID creation
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<TweetBlog> TweetBlogs { get; set; }

        // constructor that uses default db context options inherting from the base options
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Overridden method to modify default identity configurations
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
