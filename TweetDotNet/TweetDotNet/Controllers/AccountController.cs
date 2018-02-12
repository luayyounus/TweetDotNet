﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TweetDotNet.Models;
using TweetDotNet.Models.AccountViewModels;
using TweetDotNet.Services;

namespace TweetDotNet.Controllers
{
    // Require this class to use authentication to access data
    [Authorize(Roles = ApplicationRoles.Member)]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        // Injecting User manager and Sign in APIs to be used in the Account controller
        // User Manager and Sign in Manager classes are provided by Identity Framework 
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        // Interfaces injections for email info and logging users activity
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        // main constructor initializing the injected dependencies
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        // Attribute to pass in data in one http request
        // Error resets to null when another request is made that doesn't specify the string value
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///  Default Login action that uses Identity 
        /// </summary>
        /// <param name="returnUrl">overload to return to same page before logging in</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// Default Login mechanism provided by Identity framework with two overloads
        /// </summary>
        /// <param name="model">View model for login options</param>
        /// <param name="returnUrl">Return url after logging in</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        
        // Validating the source of login by storing a token in cookies to prevent cyber/unauthorized attacks from data stealth.
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            // Passing in the return url between view to keep persistency after login
            ViewData["ReturnUrl"] = returnUrl;

            // Checking if user has filled the form using the Model
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                // If user has signed in successfully, the succeeded flag will be true
                if (result.Succeeded)
                {
                    // Logging the user login details for future backlog reference
                    _logger.LogInformation("User logged in.");

                    // return to the view with the url specified earlier after login
                    return RedirectToLocal(returnUrl);
                }

                // checking if the user requires 2 factor authentication for login
                if (result.RequiresTwoFactor)
                {
                    // Redirecting with action to the specifid controller using the full assembly directory with (nameof)
                    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                }

                // checking if user has done enough attempts and blocks their account
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    // return the current model if the user has made an invalid login attempt
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Login with Two factor authentication
        /// </summary>
        /// <param name="rememberMe">Optional remember me option to remember user</param>
        /// <param name="returnUrl">Return page url after login with 2 factor auth</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            // Creating the new view model with login info and return back to the view
            var model = new LoginWith2faViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        /// <summary>
        /// Login with Two factor authentication, takes in different view model
        /// </summary>
        /// <param name="model">Two factor auth view model</param>
        /// <param name="rememberMe">Optional flag to remember logged in user</param>
        /// <param name="returnUrl">Return url after logging in</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // replacing the 2fa space with '-' because that's how it's generated usually on the screen
            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            // Getting result by passing in a 2fa, remember me option
            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        /// <summary>
        /// This action is used when user forgets user/password and needs to recover their profile
        /// </summary>
        /// <param name="model">User model with Recovery code retrieved by Email or Sms</param>
        /// <param name="returnUrl">Url return to page if recovery succeeded</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            //Checking if the recovery code
            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View();
            }
        }

        /// <summary>
        /// This is called when the user has been locked out of their account when failed recovery
        /// </summary>
        /// <returns>Return view representing account lock out</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        /// <summary>
        /// This action is called to register new user
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// Registering new users with their model information then return to page before registeration
        /// </summary>
        /// <param name="model">Model with registrar info</param>
        /// <param name="returnUrl">Url redirection after regiseration is complete</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Create new application user with Identity using the entered user email
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

                // Checks if the email already exist or available to be taken
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Create a Claim for the Email Address
                    Claim fullnameClaim = new Claim(ClaimTypes.GivenName, model.FullName, ClaimValueTypes.String);
                    Claim emailClaim = new Claim(ClaimTypes.Email, model.Email, ClaimValueTypes.String);
                    await _userManager.AddToRoleAsync(user, ApplicationRoles.Member);

                    List<Claim> memberClaims = new List<Claim> { fullnameClaim, emailClaim };
                    var claimsTask = await _userManager.AddClaimsAsync(user, memberClaims);

                    if (claimsTask.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        // Generating token info to confirm registration and prevent fraud
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        // This is a callback method the token, code generaated 
                        // and scheme to confirm request coming from the same computer/ip/device
                        var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);

                        // Send the email confirmation to the registerar
                        await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                        // once signed up successfully, Sign user in
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created a new account with password.");

                        // Return to page before registeration
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// This action is called to register new Admin 
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = ApplicationRoles.Admin)]
        [AllowAnonymous]
        public IActionResult RegisterAdmin(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = ApplicationRoles.Admin)]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAdmin(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // Create new application user with Identity using the entered user email
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

                // Checks if the email already exist or available to be taken
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Create a Claim for the Email Address
                    Claim fullnameClaim = new Claim(ClaimTypes.GivenName, model.FullName, ClaimValueTypes.String);
                    Claim emailClaim = new Claim(ClaimTypes.Email, model.Email, ClaimValueTypes.String);
                    await _userManager.AddToRoleAsync(user, ApplicationRoles.Admin);

                    List<Claim> adminClaims = new List<Claim> { fullnameClaim, emailClaim };
                    var claimsTask = await _userManager.AddClaimsAsync(user, adminClaims);

                    if (claimsTask.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        // Generating token info to confirm registration and prevent fraud
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        // This is a callback method the token, code generaated 
                        // and scheme to confirm request coming from the same computer/ip/device
                        var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);

                        // Send the email confirmation to the registerar
                        await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                        // once signed up successfully, Sign user in
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created a new account with password.");

                        // Return to page before registeration
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        /// <summary>
        /// Logout method with redirection to Home controller main index page
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        /// <summary>
        /// This method is used when login is required by third party application/website
        /// </summary>
        /// <param name="provider">Origin of account requesting login</param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        /// <summary>
        /// Callback method executed after external account has been authenticated successfully
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="remoteError"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        /// <summary>
        /// Reseting password by Model whether by Email or another form of identification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // checking if email exist in the datbase
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            // Checking user's password 
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        /// <summary>
        /// Reset password view returned when requested by reset
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
