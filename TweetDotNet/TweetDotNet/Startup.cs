using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TweetDotNet.Data;
using TweetDotNet.Models;
using TweetDotNet.Services;

namespace TweetDotNet
{
    public class Startup
    {
        // Startup constructor initializing the injected configuration dependency
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Adding dbcontext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Enable/Add Identity Framework to services to be used when startup
            // Map Application user with all its defined methods with Identity role which generates a GUID
            // Chaining Entity framework with Dbcontext and enable provide tokens for user operations
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add policy for account
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AccountRequired", policy => policy.RequireRole("Member", "Admin").Build());
            });

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            // Enabling MVC as a service in the startup process
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, RoleManager<IdentityRole> roleManager)
        {
            // checking if the app env is in developement
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                // Redirect to Error page when not in developement environment
                app.UseExceptionHandler("/Home/Error");
            }

            // Enable static files middleware to access external untrusted files added to the solution
            app.UseStaticFiles();

            // Enable authentication middleware to be used across the assembly files of the solution
            app.UseAuthentication();

            // default mvc app route to index action
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Role Creation using role Manager to populate database
            var checkMember = roleManager.RoleExistsAsync(ApplicationRoles.Member);
            checkMember.Wait();
            if (!checkMember.Result)
            {
                var member = roleManager.CreateAsync(
                new IdentityRole
                {
                    Name = ApplicationRoles.Member,
                    NormalizedName = ApplicationRoles.Member
                });
                member.Wait();
            }

            var checkAdmin = roleManager.RoleExistsAsync(ApplicationRoles.Admin);
            checkAdmin.Wait();
            if (!checkAdmin.Result)
            {
                var admin = roleManager.CreateAsync(
                new IdentityRole
                {
                    Name = ApplicationRoles.Admin,
                    NormalizedName = ApplicationRoles.Admin
                });
                admin.Wait();
            }
        }
    }
}
