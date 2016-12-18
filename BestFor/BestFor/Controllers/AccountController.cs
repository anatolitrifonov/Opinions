using BestFor.Domain.Entities;
using BestFor.Dto;
using BestFor.Dto.Account;
using BestFor.Models;
using BestFor.Services;
using BestFor.Services.Blobs;
using BestFor.Services.Messaging;
using BestFor.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BestFor.Controllers
{
    /// <summary>
    /// Controller that handles user registration and logins.
    /// 
    /// This filter will parse the culture from the URL and set it into Viewbag.
    /// Controller has to inherit BaseApiController in order for filter to work correctly.
    /// </summary>
    [ServiceFilter(typeof(LanguageActionFilter))]
    [Authorize]
    public class AccountController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        private readonly IProfanityService _profanityService;
        private readonly IResourcesService _resourcesService;
        private readonly IBlobService _blobService;
        private readonly IStatisticsService _statisticsService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender, ISmsSender smsSender, IUserService userService, IProfanityService profanityService,
            ILoggerFactory loggerFactory, IResourcesService resourcesService, IBlobService blobService,
            IStatisticsService statisticsService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _userService = userService;
            _profanityService = profanityService;
            _resourcesService = resourcesService;
            _blobService = blobService;
            _statisticsService = statisticsService;
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var model = new LoginViewDto() { ReturnUrl = returnUrl };
            return View(model);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewDto model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                // User is cancelled -> Go to home page.
                if (user != null && user.IsCancelled)
                {
                    return RedirectToAction("Index", "Home");
                }
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/AccessDenied
        /// <summary>
        /// Shown when user hit an unauthorized url
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // GET: /Account/SendCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewDto { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // redisplay the form if something is wrong with model.
            if (!ModelState.IsValid) return View(model);

            // Do profanity checks. We already validated the model.
            if (!IsProfanityCleanProfileCreate(model)) return View(model);

            // Check email is unique.
            if (!await IsEmailUnique(model.Email, null)) return View(model);

            // Check display name is unique.
            if (!IsDisplayNameUnique(model.DisplayName, null)) return View(model);

            // Username will be checked by user manager.
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                DisplayName = model.DisplayName,
                DateAdded = DateTime.Now,
                DateUpdated = DateTime.Now
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                //    "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation(3, "User created a new account with password.");

                // Add user to cache
                _userService.AddUserToCache(user.ToDto());

                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            AddErrors(result);

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewDto model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null) // || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                _emailSender.SendEmailAsync(model.Email, "Reset Password",
                   "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// Confirms that forgot password reset email was sent
        /// GET: /Account/ForgotPasswordConfirmation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// This is a password reset from forgotten password link.
        /// Shows the view for the link that was sent to the user by email
        /// Link contains the code to reset the password.
        /// GET: /Account/ResetPassword
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        /// <summary>
        /// Posts the new password and reset code
        /// POST: /Account/ResetPassword
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            AddErrors(result);
            return View();
        }

        /// <summary>
        /// Shows that account reset was done.
        /// GET: /Account/ResetPasswordConfirmation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// GET: /Account/ViewProfile
        /// Has to be logged in.
        /// Display name can be blank or unique.
        /// Can not change user name.
        /// Can not change password.
        /// Have to type password to confirm update.
        /// </summary>
        /// <returns></returns>
        /// <remarks>This is just a get to load current profile</remarks>
        [HttpGet]
        public IActionResult ViewProfile()
        {
            var currentUserId = _userManager.GetUserId(User);

            // User manager will go to database to find info about the user.
            // Let's go to cache.
            // we are authenticated so currentUserId will be there.
            var user = _userService.FindById(currentUserId);
            if (user == null)
            {
                // Would be funny if this happens. Someone is hacking us I guess.
                return View("Error");
            }
            var model = new ProfileEditViewModel();
            model.Email = user.Email;
            model.UserName = user.UserName;
            model.DisplayName = user.DisplayName;

            model.JoinDate = user.DateAdded;
            model.PhoneNumber = user.PhoneNumber;
            model.CompanyName = user.CompanyName;
            model.WebSite = user.WebSite;
            model.UserDescription = user.UserDescription;
            model.ShowEmail = user.ShowEmail;
            model.ShowPhoneNumber = user.ShowPhoneNumber;
            model.ShowCompanyName = user.ShowCompanyName;
            model.ShowWebSite = user.ShowWebSite;
            model.ShowUserDescription = user.ShowUserDescription;
            model.ShowAvatar = user.ShowAvatar;
            model.Level = user.Level;

            // Check and update stats
            _statisticsService.LoadUserStatictics(user);

            model.NumberOfAnswers = user.NumberOfAnswers;
            model.NumberOfDescriptions = user.NumberOfDescriptions;
            model.NumberOfVotes = user.NumberOfVotes;
            model.NumberOfFlags = user.NumberOfFlags;

            model.NumberOfComments = user.NumberOfComments;

            // Populate image, load users image if needed
            model.UserImageUrl = _blobService.GetUserImagUrl(user);
            model.UserImageUrlSmall = user.UserImageUrlSmall;

            return View(model);
        }

        /// <summary>
        /// GET: /Account/EditProfile
        /// Has to be logged in.
        /// Can only change email and display name. Display name can be blank or unique.
        /// Can not change user name.
        /// Can not change password.
        /// Have to type password to confirm update.
        /// </summary>
        /// <returns></returns>
        /// <remarks>This is just a get to load current profile</remarks>
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var currentUserId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(currentUserId);
            if (user == null)
            {
                // Would be funny if this happens. Someone is hacking us I guess.
                return View("Error");
            }
            var model = new ProfileEditViewModel();
            model.Email = user.Email;
            model.UserName = user.UserName;
            model.DisplayName = user.DisplayName;
            model.NumberOfAnswers = user.NumberOfAnswers;
            model.NumberOfDescriptions = user.NumberOfDescriptions;
            model.NumberOfVotes = user.NumberOfVotes;
            model.NumberOfFlags = user.NumberOfFlags;
            model.NumberOfComments = user.NumberOfComments;
            model.JoinDate = user.DateAdded;
            model.PhoneNumber = user.PhoneNumber;
            model.CompanyName = user.CompanyName;
            model.WebSite = user.WebSite;
            model.UserDescription = user.UserDescription;
            model.ShowEmail = user.ShowEmail;
            model.ShowPhoneNumber = user.ShowPhoneNumber;
            model.ShowCompanyName = user.ShowCompanyName;
            model.ShowWebSite = user.ShowWebSite;
            model.ShowUserDescription = user.ShowUserDescription;
            model.ShowAvatar = user.ShowAvatar;

            return View(model);
        }

        /// <summary>
        /// POST: /Account/EditProfile
        /// Update user's profile. Does not navigate away.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ProfileEditViewModel model)
        {
            // Check the model first
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Find id
            var currentUserId = _userManager.GetUserId(User);
            // Find user
            var user = await _userManager.FindByIdAsync(currentUserId);
            if (user == null)
            {
                // Would be funny if this happens. Someone is hacking us I guess.
                return View("Error");
            }

            // First check if there was any changes
            if (!IsChanged(model, user))
            {
                // No changes
                model.SuccessMessage = "Your profile was successfully updated.";
                return View(model);
            }

            // Do profanity checks. We already validated the model.
            if (!IsProfanityCleanProfileUpdate(model)) return View(model);

            // Verify password
            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                ModelState.AddModelError(string.Empty, "Invalid password");
                return View(model);
            }

            user.DisplayName = model.DisplayName;
            user.Email = model.Email;

            // Check email is unique.
            if (!await IsEmailUnique(model.Email, user.Id)) return View(model);

            // Check display name is unique.
            if (!IsDisplayNameUnique(model.DisplayName, user.Id)) return View(model);

            user.PhoneNumber = model.PhoneNumber;
            user.CompanyName = model.CompanyName;
            user.WebSite = model.WebSite;
            user.UserDescription = model.UserDescription;
            user.ShowEmail = model.ShowEmail;
            user.ShowPhoneNumber = model.ShowPhoneNumber;
            user.ShowCompanyName = model.ShowCompanyName;
            user.ShowWebSite = model.ShowWebSite;
            user.ShowUserDescription = model.ShowUserDescription;
            user.ShowAvatar = model.ShowAvatar;

            var updateResult = await _userManager.UpdateAsync(user);

            // If all good we stay on the same page and display success message.
            if (updateResult.Succeeded)
            {
                model.SuccessMessage = "Your profile was successfully updated.";
            }

            // Need to update cache .....
            _userService.AddUserToCache(user.ToDto());

            AddErrors(updateResult);

            return View(model);
        }

        /// <summary>
        /// Compares data in application user object and in edit user model to detect the change.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool IsChanged(ProfileEditViewModel model, ApplicationUser user)
        {
            bool same = model.DisplayName == user.DisplayName &&
                user.Email == model.Email &&
                user.PhoneNumber == model.PhoneNumber &&
                user.CompanyName == model.CompanyName &&
                user.WebSite == model.WebSite &&
                user.UserDescription == model.UserDescription &&
                user.ShowEmail == model.ShowEmail &&
                user.ShowPhoneNumber == model.ShowPhoneNumber &&
                user.ShowCompanyName == model.ShowCompanyName &&
                user.ShowWebSite == model.ShowWebSite &&
                user.ShowUserDescription == model.ShowUserDescription &&
                user.ShowAvatar == model.ShowAvatar;

            return !same;
        }

        /// <summary>
        /// GET: /Account/ChangePassword
        /// Has to be logged in.
        /// Can only change password.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ChangePassword()
        {
            // Nothing to show on this page user has to enter everything manually.
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            // Check the model first
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Find id
            var currentUserId = _userManager.GetUserId(User);
            // Find user
            var user = await _userManager.FindByIdAsync(currentUserId);
            if (user == null)
            {
                // Would be funny if this happens. Someone is hacking us I guess.
                return View("Error");
            }

            // Verify password
            if (!await _userManager.CheckPasswordAsync(user, model.OldPassword))
            {
                ModelState.AddModelError(string.Empty, "Invalid password");
                return View(model);
            }

            var updateResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            // If all good we stay on the same page and display success message.
            if (updateResult.Succeeded)
            {
                model.SuccessMessage = "Your password was successfully updated.";
            }

            AddErrors(updateResult);

            return View(model);
        }

        /// <summary>
        /// Render page for avatar upload
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult UploadAvatar()
        {
            var model = new AdminUploadImage();
            model.ImageForUserName = _userManager.GetUserName(User);
            return View(model);
        }

        /// <summary>
        /// Image upload handling. This is not a full post/rerender page call.
        /// Post happens throught javascript.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UploadAvatar(AdminUploadImage model)
        {
            // todo this method is currently not quite protected 
            // however controller itself is marked as Authorize.
            if (model == null) return FormJsonResultError("empty model");
            if (string.IsNullOrEmpty(model.ImageForUserName) || string.IsNullOrWhiteSpace(model.ImageForUserName))
                return FormJsonResultError("empty user name");
            if (model.TheImageToUpload == null)
                return FormJsonResultError("empty upload file");
            if (model.ImageForUserName != _userManager.GetUserName(User))
                return FormJsonResultError("invalid user");

            var blobData = new BlobDataDto()
            {
                FileName = model.TheImageToUpload.FileName,
                Stream = model.TheImageToUpload.OpenReadStream()
            };

            try
            {
                // Save image into the blob storage.
                _blobService.SaveUserProfilePicture(model.ImageForUserName, blobData);
            }
            catch (ServicesException ex)
            {
                return FormJsonResultError(ex.Message);
            }

            // cache the user
            var user = _userService.FindAll().FirstOrDefault(x => x.UserName == model.ImageForUserName);
            if (user != null) _blobService.SetUserImageCached(user, true);

            return FormJsonResult("File " + blobData.FileName + " uploaded successfully.", "");
        }

        /// <summary>
        /// Deletes user's avatar.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult DeleteAvatar()
        {
            // Get user
            var user = _userService.FindByUserName(User.Identity.Name);

            // Load blob info
            _blobService.GetUserImagUrl(user);

            // Delete only if there
            if (user.UserImageUrl != null)
            {
                _blobService.DeleteUserProfilePicture(user.UserName);
                user.UserImageUrl = null;
            }

            return RedirectToAction("ViewProfile");
        }

        [HttpGet]
        public async Task<IActionResult> CancelProfile()
        {
            var currentUserId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(currentUserId);
            if (user == null)
            {
                // Would be funny if this happens. Someone is hacking us I guess.
                return View("Error");
            }
            var model = new RemoveProfileViewModel();
            model.UserName = user.UserName;
            model.DisplayName = user.DisplayName;

            return View(model);
        }

        /// <summary>
        /// POST: /Account/Profile
        /// Update user's profile. Does not navigate away.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelProfile(RemoveProfileViewModel model)
        {
            // Check the model first
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Find id
            var currentUserId = _userManager.GetUserId(User);
            // Find user
            var user = await _userManager.FindByIdAsync(currentUserId);
            if (user == null)
            {
                // Would be funny if this happens. Someone is hacking us I guess.
                return View("Error");
            }

            user.IsCancelled = true;
            user.DateUpdated = DateTime.Now;
            user.DateCancelled = DateTime.Now;
            user.CancellationReason = model.Reason;

            var updateResult = await _userManager.UpdateAsync(user);

            // Log out.
            await _signInManager.SignOutAsync();

            // If all good we redirect to home.
            if (updateResult.Succeeded)
            {
                // Read the reason
                var reason = _resourcesService.GetString(this.Culture, Lines.YOUR_PROFILE_WAS_REMOVED);
                return RedirectToAction("Index", "Home", new { reason = reason });
            }

            AddErrors(updateResult);

            return View(model);
        }

        #region Private Methods

        /// <summary>
        /// Put all errors from identity result into model state.
        /// </summary>
        /// <param name="result"></param>
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        //private async Task<ApplicationUser> GetCurrentUserAsync()
        //{
        //    return await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
        //}

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

        /// <summary>
        /// Check profile update data for profanity.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool IsProfanityCleanProfileUpdate(ProfileEditViewModel model)
        {
            // Do profanity checks. We already validated the model.
            // we can only change a couple of fields.
            var result =  _profanityService.CheckProfanity(model.DisplayName);
            // Disaply name can be blank.
            if (!string.IsNullOrEmpty(model.DisplayName) && !string.IsNullOrWhiteSpace(model.DisplayName))
                if (result.HasIssues)
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);

            return ModelState.ErrorCount == 0;
        }

        /// <summary>
        /// Check profile creation data for profanity.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool IsProfanityCleanProfileCreate(RegisterViewModel model)
        {
            // Do profanity checks. We already validated the model.
            // we can only change a couple of fields.
            var result = _profanityService.CheckProfanity(model.DisplayName);
            // Disaply name can be blank.
            if (!string.IsNullOrEmpty(model.DisplayName) && !string.IsNullOrWhiteSpace(model.DisplayName))
                if (result.HasIssues)
                    ModelState.AddModelError(string.Empty, result.ErrorMessage);

            // We do want to check the username since it is displayable
            result = _profanityService.CheckProfanity(model.UserName);
            if (result.HasIssues)
                ModelState.AddModelError(string.Empty, result.ErrorMessage);

            // We are not showing email anywhere so let's let users pick whatever the hell they want
            //result = _profanityService.CheckProfanity(model.Email);
            //if (result.HasIssues)
            //    ModelState.AddModelError(string.Empty, result.ErrorMessage);

            return ModelState.ErrorCount == 0;
        }

        /// <summary>
        /// Check user's email uniqueness using user manager.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="userId">existing/current userid to skip</param>
        /// <returns></returns>
        private async Task<bool> IsEmailUnique(string email, string userId)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                if (userId == user.Id)
                    return true; // ignore existing/current user

                ModelState.AddModelError(string.Empty, "This email is already taken.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check display name uniqueness using user service.
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="userId">existing userid to skip</param>
        /// <returns></returns>
        private bool IsDisplayNameUnique(string displayName, string userId)
        {
            // blank Display name is OK
            if (string.IsNullOrEmpty(displayName) || string.IsNullOrWhiteSpace(displayName)) return true;
            var user = _userService.FindDirectByDisplayName(displayName);
            if (user != null)
            {
                if (userId == user.UserId)
                    return true; // ignore existing/current user

                ModelState.AddModelError(string.Empty, "This display name is already taken.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Shortcut for creating small json object with a message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private IActionResult FormJsonResultError(string error)
        {
            return FormJsonResult("", error);
        }

        private IActionResult FormJsonResult(string message, string error)
        {
            return Json(new { Message = message, Error = string.IsNullOrEmpty(error) ? "" : "Error: " + error });
        }
        #endregion
    }
}
