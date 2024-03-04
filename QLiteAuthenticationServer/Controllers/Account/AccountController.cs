using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using QLite.Data.Models.Auth;
using QLite.Data.ViewModels;
using QLite.Data;
using QLiteAuthenticationServer.Context;
using QLiteAuthenticationServer.Helpers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QLite.Data.ViewModels.Login;
using QLite.Data.ViewModels.Account;
using QLite.Data.ViewModels.Logout;

namespace QLiteAuthenticationServer.Controllers.Account
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly EmailSender _emailSender;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,
            IConfiguration configuration,
            ApplicationDbContext Context,
            EmailSender emailSender
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
            _configuration = configuration;
            _context = Context;
            _emailSender = emailSender;
        }

        #region EMAIL MANAGEMENT

        /// <summary>
        /// This handler takes email confirmation token along with the user id and confirms the email of the user. 
        /// Users are sent an email confirmation email upon account creation and when they request to change their email. They land here when they click the link.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="emailResetToken">Email confirmation token</param>
        /// <returns></returns>
        public async Task<IActionResult> ConfirmEmail(string userId, string newEmail, string emailResetToken)
        {
            #region VALIDATIONS
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newEmail) || string.IsNullOrEmpty(emailResetToken))
                return View("Error", new ErrorViewModel { simpleErrorMessage = "Bad Request." });

            // check if user exists
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return View("Error", new ErrorViewModel { simpleErrorMessage = "User does not exist." });

            // check if email is valid
            if (!Utils.IsValidEmail(newEmail))
                return View("Error", new ErrorViewModel { simpleErrorMessage = "Email is invalid." });
            #endregion

            //var emailChangeResult = _userManager.ConfirmEmailAsync(user, emailResetToken); this is not working sometimes for some reason
            var emailChangeResult = await _userManager.ChangeEmailAsync(user: user, newEmail: newEmail, token: emailResetToken); // this method validates the token

            if (emailChangeResult.Succeeded || (user.EmailConfirmed == true && user.Email == newEmail))
            {
                // in our system, we keep usernames and emails synced, so we must also update these fields when changing email.
                if (user.UserName != newEmail)
                {
                    user.UserName = newEmail;
                    user.NormalizedUserName = newEmail.ToUpper();
                    if ((await _context.SaveChangesAsync()) != 1)
                        return View("Error", new ErrorViewModel { simpleErrorMessage = "An unexpected error occured when updating user info." });
                }

                // accounts are created without any password, in which case, a random password must be assigned to the account
                // this "temporary" password is NOT emailed to the user
                // instead, a password change token is created with this password (because a password is needed for generating a token).
                // a link to change password is sent to the user, including user ID and token in query string
                // therefore users are not required to be logged in to change their password with this link

                if (string.IsNullOrEmpty(user.PasswordHash)) // is the account a new account?
                {
                    #region INITIALIZE PASSWORD & GENERATE TOKEN
                    string tempPw = await Utils.GenerateRandomPassword();
                    string tempInternalResetToken = await _userManager.GeneratePasswordResetTokenAsync(user); // this token is only used to internally set user password.

                    if (string.IsNullOrEmpty(tempInternalResetToken))
                        return View("Error", new ErrorViewModel { simpleErrorMessage = "Unable to initialize password. Please ask your administrator to reset your password." });

                    var pwResetResult = await _userManager.ResetPasswordAsync(user, tempInternalResetToken, tempPw);
                    if (!pwResetResult.Succeeded)
                        return View("Error", new ErrorViewModel { simpleErrorMessage = "Unable to initialize password. Please ask your administrator to reset your password." });

                    var pwChangeToken = await Utils.GeneratePasswordChangeToken(userId: userId, currentPassword: user.PasswordHash); // generate custom password change token
                    #endregion

                   // #region SEND EMAIL
                   // var link = _configuration["SiteDomain"] + "/Account/Initialize?userId=" + user.Id + "&passwordChangeToken=" + pwChangeToken;

                   // var fullBody = new TextPart("html")
                   // {
                   //     Text = $@"<!DOCTYPE html>
                   //         <html>
                   //             <body>
                   //                 <p>Thank you for confirming your email. As you have a new account, there are additional steps to complete your setup. Please <a style='font-weight:bold;' href='{link}'>click here</a> to establish your password and set up 2FA. 
                   //                 </p>
                   //             </body>
                   //         </html>"
                   // };

                   // var email = new Email
                   // {
                   //     body = fullBody,
                   //     subject = "Let's Finish Setting Up Your Account",
                   //     toAddress = user.Email
                   // };

                   //// _ = _emailSender.AddJobAsync(email); // fire and forget. do not await.
                   // #endregion
                }

                _ = _signInManager.RefreshSignInAsync(user);

                return View("Callback/EmailConfirmed");
            }
            else
            {
                // TODO logla
                return View("Error", new ErrorViewModel { simpleErrorMessage = "Unable to confirm email." });
            }
        }

        #endregion


        #region ACCOUNT INITIALIZATION

        // change password and set up 2FA at once. meant only to be used once by new accounts
        // doesn’t require to be logged in, requires password change token. sets password and 2FA at once
        // new accounts are be automatically sent a link to initialize their account here

        /// <summary>
        /// This page requires user to change password AND set up 2FA. Users are sent a link to this page when they confirmed their email for the first time, or when they lost access to their 2FA.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Initialize(string userId, string passwordChangeToken)
        {
            #region VALIDATIONS
            var _user = await _userManager.FindByIdAsync(userId);
            if (_user == null)
                return View("Error", new ErrorViewModel { simpleErrorMessage = "User not found." });

            if (!await Utils.ValidatePasswordChangeToken(userId: userId, tokenToValidate: passwordChangeToken, currentPassword: _user.PasswordHash))
                return View("Error", new ErrorViewModel { simpleErrorMessage = "Invalid token." });
            #endregion

            #region GENERATE QR IMAGE (IF USER IS NOT QRLESS)
            bool QRless = _user.QRless;
            string encodedQR = string.Empty;
            if (!QRless)
                encodedQR = await Utils.GenerateQRImage(_user);
            #endregion

            return View("Initialize", new AccountSetupModel { PasswordChangeToken = passwordChangeToken, EncodedQR = encodedQR, QRless = QRless });
        }

        /// <summary>
        /// This action handles the AJAX post requests made from /Account/Initialize
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PerformInitialization(AccountSetupModel model, string passwordChangeToken)
        {
            #region VALIDATIONS
            // attempt to get the relevant user
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return Json(new { success = false, error = "User not found." });

            // validate if password exists, and if password confirmation is correct
            if (string.IsNullOrEmpty(model.NewPassword) || (model.NewPassword != model.ConfirmNewPassword))
                return Json(new { success = false, error = "Password mismatch, try again." });

            // validate the custom password change token
            if (!await Utils.ValidatePasswordChangeToken(userId: user.Id, tokenToValidate: passwordChangeToken, currentPassword: user.PasswordHash))
                return Json(new { success = false, error = "Invalid token." });

            // validate OTP
            if (!user.QRless)
                if (!Utils.ValidateOTP(userTwoFactorSecret: user.TwoFactorSecret, code: model.OTP))
                    return Json(new { success = false, error = "Invalid OTP code, try again." });
            #endregion

            try
            {
                #region SET PASSWORD
                // this token (tempInternalResetToken) is only used internally here to set user password.
                string tempInternalResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, tempInternalResetToken, model.NewPassword);
                if (result == null || result.Succeeded == false)
                    return Json(new { success = false, error = result.Errors.FirstOrDefault().Description });
                #endregion

                #region SET 2FA AS CONFIRMED (IF USER IS NOT QRLESS)
                if (!user.QRless)
                {
                    user.TwoFactorConfirmed = true;
                    if ((await _context.SaveChangesAsync()) == 1)
                        return Json(new { success = true });
                    else
                        return Json(new { success = false, error = "Unable to persist changes to the database." });
                }
                else
                    return Json(new { success = true });
                #endregion
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        #endregion


        #region ADMIN 2FA RESET

        // when admins reset 2FA of a user, users are sent a link to this page
        // this page does not require to be logged in, as it is meant for users who lost their 2FA
        // security is achieved with the user id passed via query string in URL
        // users' 2FA state is checked before view is returned, thus rendering used email links useless

        [HttpGet]
        public async Task<IActionResult> Reset2FA(string userId)
        {
            #region VALIDATIONS
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                return View("Error", new ErrorViewModel { simpleErrorMessage = "User not found." });

            if (user.TwoFactorConfirmed)
                return View("Error", new ErrorViewModel { simpleErrorMessage = "User has already confirmed their 2FA." });

            if (user.QRless)
                return View("Error", new ErrorViewModel { simpleErrorMessage = "User is QRless. Please contact your administrator to update your account for 2FA." });
            #endregion

            // generate a new OTP secret
            var NewTwoFactorSecret = Utils.GenerateTwoFactorKey();

            // update user's qr-related properties
            user.TwoFactorSecret = NewTwoFactorSecret;
            user.TwoFactorConfirmed = false;

            // persist changes to db
            if ((await _context.SaveChangesAsync()) != 1)
                return View("Error", new ErrorViewModel { simpleErrorMessage = "An unexpected error occured when updating user info." });

            // finally, generate QR image based on updated user info
            string encodedQR = await Utils.GenerateQRImage(user);

            return View("Reset2FA", new AccountSetupModel { EncodedQR = encodedQR });
        }

        /// <summary>
        /// This action handles the AJAX post requests made from /Account/Reset2FA
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Perform2FAReset(AccountSetupModel model)
        {
            #region VALIDATIONS
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);
            if (user == null)
                return Json(new { success = false, error = "User not found." });

            if (user.TwoFactorConfirmed)
                return Json(new { success = false, error = "User has already confirmed their 2FA." });

            if (user.QRless)
                return Json(new { success = false, error = "User is QRless." });

            if (!Utils.ValidateOTP(userTwoFactorSecret: user.TwoFactorSecret, code: model.OTP))
                return Json(new { success = false, error = "Invalid OTP." });
            #endregion

            #region UPDATE USER
            try
            {
                user.TwoFactorConfirmed = true;
                await _context.SaveChangesAsync();
                // assumes all is good
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // TODO logla
                return Json(new { success = false, error = "An unexpected error happened." });
            }
            #endregion
        }

        #endregion


        #region ADMIN PASSWORD RESET

        // when admins reset the password of a user, users are sent a link to this page
        // this page does not require to be logged in, as it is meant for users who forgot their password
        // security is achieved with the user id and token passed via query string in URL
        // NOTE: users can change their password on their own if they can log in to the system at `Account/MyProfile`

        [HttpGet]
        public async Task<IActionResult> ChangePassword(string userId, string passwordChangeToken)
        {
            try
            {
                #region VALIDATIONS
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return View("Error", new ErrorViewModel { simpleErrorMessage = "User not found." });

                if (!await Utils.ValidatePasswordChangeToken(userId: userId, tokenToValidate: passwordChangeToken, currentPassword: user.PasswordHash))
                    return View("Error", new ErrorViewModel { simpleErrorMessage = "Invalid token." });
                #endregion

                return View("ChangePassword", new AccountSetupModel { PasswordChangeToken = passwordChangeToken });
            }
            catch (Exception ex)
            {
                //TODO logla 
                return View("Error", new ErrorViewModel { simpleErrorMessage = "An unexpected error happened." });
            }
        }

        /// <summary>
        /// This action handles the AJAX post requests made from /Account/ChangePassword
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PerformPasswordChange(AccountSetupModel model, string passwordChangeToken)
        {
            #region VALIDATIONS
            if (string.IsNullOrEmpty(model.NewPassword) || (model.NewPassword != model.ConfirmNewPassword))
                return Json(new { success = false, error = "Password mismatch, try again." });

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return Json(new { success = false, error = "User not found." });

            if (!await Utils.ValidatePasswordChangeToken(userId: user.Id, tokenToValidate: passwordChangeToken, currentPassword: user.PasswordHash))
                return Json(new { success = false, error = "Invalid token." });
            #endregion

            #region CHANGE PASSWORD
            try
            {
                string tempInternalResetToken = await _userManager.GeneratePasswordResetTokenAsync(user); // this token is only used to internally set user password.
                var result = await _userManager.ResetPasswordAsync(user, tempInternalResetToken, model.NewPassword);
                if (!result.Succeeded)
                    return Json(new { success = false, error = result.Errors.FirstOrDefault().Description });

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
            #endregion
        }

        #endregion


        #region MESSAGE DISPLAY (CALLBACK VIEWS)

        // we redirect users to these pages instead of showing them popup messages because we don't want users to stay on account management pages after their operation is completed.
        // tokens expire after they are used, and we don't want users to potentially attempt re-submitting the forms with the same token, resulting in "invalid token" errors

        public IActionResult AccountInitialized() => View("Callback/AccountInitialized");
        public IActionResult EmailChanged() => View("Callback/EmailChanged");
        public IActionResult EmailConfirmed() => View("Callback/EmailConfirmed");
        public IActionResult PasswordChanged() => View("Callback/PasswordChanged");
        public IActionResult TwoFactorChanged() => View("Callback/TwoFactorChanged");
        public IActionResult ProfileUpdated() => View("Callback/ProfileUpdated");

        #endregion


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> InvalidFingerprint()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync();
                await _signInManager.SignOutAsync();
                return RedirectToAction("InvalidFingerprint");
            }
            else
                return View("Callback/InvalidFingerprint");
        }


        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            // build a model so we know what to show on the login page
            var vm = await BuildLoginViewModelAsync(returnUrl);

            return View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model, string button)
        {
            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            if (string.IsNullOrEmpty(model.ReturnUrl))
                model.ReturnUrl = _configuration["SiteDomain"];

            // the user clicked the "cancel" button
            if (button != "login")
            {
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    if (context.IsNativeClient())
                    {
                        // The client is native, so this change in how to
                        // return the response is for better UX for the end user.
                        return this.LoadingPage("Redirect", model.ReturnUrl);
                    }

                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    // since we don't have a valid context, then we just go back to the home page
                    //return Redirect("~/");
                    return Redirect(_configuration["SiteDomain"] == null ? "~/" : _configuration["SiteDomain"]);
                }
            }

            if (ModelState.IsValid)
            {
                #region VALIDATIONS
                // check if user exists
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "User not found.", clientId: context?.Client.ClientId));
                    ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);

                    // something went wrong, show form with error
                    var vm2 = await BuildLoginViewModelAsync(model);
                    return View(vm2);
                }

                // check if user is active
                if (!user.IsActive)
                {
                    await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "This account is inactive.", clientId: context?.Client.ClientId));
                    ModelState.AddModelError(string.Empty, "This account is inactive");

                    // something went wrong, show form with error
                    var vm2 = await BuildLoginViewModelAsync(model);
                    return View(vm2);
                }

                // validate OTP
                if (!user.QRless) // only validate if user is not a "qrless" account
                {
                    if (!Utils.ValidateOTP(userTwoFactorSecret: user.TwoFactorSecret, code: model.OTP))
                    {
                        await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "Invalid OTP", clientId: context?.Client.ClientId));
                        ModelState.AddModelError(string.Empty, "Invalid OTP");

                        // something went wrong, show form with error
                        var vm2 = await BuildLoginViewModelAsync(model);
                        return View(vm2);
                    }
                }
                #endregion
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberLogin, lockoutOnFailure: true);

                if (!result.Succeeded)
                {

                    await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "Invalid credentials", clientId: context?.Client.ClientId));
                    ModelState.AddModelError(string.Empty, "Invalid credentials");

                    // something went wrong, show form with error
                    var vm2 = await BuildLoginViewModelAsync(model);
                    return View(vm2);

                }
                #region INITIALIZE OR UPDATE FINGERPRINT CLAIM
                var existingFingerprint = _context.UserClaims
                    .FirstOrDefault(x => x.UserId == user.Id && x.ClaimType == "Fingerprint");

                string newFingerprint = Utils.GenerateFingerprint(HttpContext);

                // if a fingerprint already exists, we must overwrite it.
                if (existingFingerprint != null)
                {
                    existingFingerprint.ClaimValue = newFingerprint;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var claim = new Claim("Fingerprint", newFingerprint); // Create the claim
                    await _userManager.AddClaimAsync(user, claim); // Add claim to user
                }
                await _signInManager.RefreshSignInAsync(user); // refresh login to reflect changes to issued cookie
                #endregion


               

                await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.Client.ClientId));

                if (context != null)
                {
                    if (context.IsNativeClient())
                    {
                        // The client is native, so this change in how to
                        // return the response is for better UX for the end user.
                        return this.LoadingPage("Redirect", model.ReturnUrl);
                    }

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    return Redirect(model.ReturnUrl);
                }


                return Redirect(model.ReturnUrl);
            }

            // something went wrong, show form with error
            var vm = await BuildLoginViewModelAsync(model);
            return View(vm);
        }


        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId, string? redirectUri)
        {
            // build a model so the logout page knows what to display
            var vm = await BuildLogoutViewModelAsync(logoutId);

            //if (vm.ShowLogoutPrompt == false)
            //{
            // if the request for logout was properly authenticated from IdentityServer, then
            // we don't need to show the prompt and can just log the user out directly.
            await HttpContext.SignOutAsync();
            return await Logout(vm);
            //}

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await _signInManager.SignOutAsync();

                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            #region CUSTOMER THY ISE ACM LOGOUT
            if (_configuration["Customer"] == "THY")
            {
                switch (_configuration["ThyEnv"])
                {
                    case "TEST":
                        return Redirect("https://logintest.thy.com/oam/server/logout");
                    case "PROD":
                        return Redirect("https://auth.thy.com/oam/server/logout");
                    default:
                        return View("Views/Home/Index.cshtml");
                }
            }
            #endregion

            if (!string.IsNullOrEmpty(vm.PostLogoutRedirectUri))
                return Redirect(vm.PostLogoutRedirectUri);
            else
                return View("LoggedOut", vm);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == IdentityServer4.IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var vm = new LoginViewModel
                {
                    EnableLocalLogin = local,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                };

                if (!local)
                {
                    vm.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
                }

                return vm;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName ?? x.Name,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.Client.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }
    }
}
