using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TweetDotNet.Models
{
    public class MinimumAgeRequirement : IAuthorizationRequirement
    {
    }

    public class Is12 : AuthorizationHandler<MinimumAgeRequirement>
    {
        private const int minAge = 12;
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth))
                return Task.CompletedTask;

            DateTime birthday = Convert.ToDateTime(context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth).Value);

            int age = DateTime.Now.Year - birthday.Year;

            if (age >= minAge)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
