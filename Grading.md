### 3 points - Create 2 different types of users (Member-type and Admin-type)
`Models` -> `Application User` -> In a Application Roles class

`Startup` -> `Configure` method using the Role manager to populate the DbTable

### 4 points - Create a different registration page for the Admin page (this can be a hidden link)
The method is created at `Controllers` -> `AccountController` -> `RegisterAdmin` method.

To register as an Admin, use the hidden link `host:/Account/RegisterAdmin`
### 4 points - Attach at least 4 claims (including their Role) to the registration process.
Claims are located at `Controllers` -> `AccountController` -> inside both `Register` and `RegisterAdmin` methods.

### 4 points - Redirect the user, dependant on their role, to a specific page, upon login (can you figure out how to lock down a page dependant on their Role?)
`Controllers` -> `AccountController` -> `Login` method directs to the same page because that's the intention of the Application. The checking though is happening on the view to let Admin modify results located inside `Views` -> `TweetBlog` -> `Edit`.

To test the feature, you have to `Sign up` once through the hidden page as an Admin, `add a blog` then view its details. The admin will be able to `delete`, `edit`. 
Now `signout` and `sign up` as a member using the home page. You will find the blog but won't have the privilige to `delete` or `edit`.

### 4 points (1 point for each CRUD operation) - Create a standard CRUD controller to manipulate data within your site.
Go to `Controllers` -> `TweetBlogController`... There are CRUD operations happening in that file.

### 4 points - Restrict your CRUD controller to specific access.
Go to `Controllers` -> `TweetBlogController` ... the whole class is restricted to users who are signed up.

### 3 points - Update your application to include at least 2 roles.
Already answered in the previous questions. Inside `Models` -> `Application User`

### 3 points - Add at least one role based policy and one custom Authorization Handler policy to your application.
Explained in the next Questions :)

### 3 points - Implement both of these policies in your site.
Go to `Controllers` -> `TweetBlogController` then look at the Class `data annotation`
```C#
[Authorize(Policy = "AccountRequired")]
```

This policies are created to restrict access based on the built-in policy in the `Startup file`

```C#
services.AddAuthorization(options =>
{
    options.AddPolicy("AccountRequired", policy => policy.RequireRole("Member", "Admin").Build());
    options.AddPolicy("MinimumAge", policy => policy.Requirements.Add(new MinimumAgeRequirement()));
});
```


and the custom policy which is created in `Models` inside `Minimum Age Requirement` and used to restrict underage users from `Advanced Topics` used inside `Controllers` -> `TweetBlogController` -> `AdvancedTopics`

The `Edit` Action is restricted based on the `role`

### 3 points - Update your application to include a View Component.
I can find this :)
