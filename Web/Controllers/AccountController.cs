using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Web.Models;
using Web.Services.EmailService;
using Web.ViewModels;

namespace Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var viewModel = new RegisterVm();
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVm registerVm, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {
                    Name = registerVm.Name,
                    Email = registerVm.Email,
                    UserName = registerVm.Email
                };

                var result = await _userManager.CreateAsync(user, registerVm.Password);
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var callBackUrl =
                    Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code },
                    protocol: HttpContext.Request.Scheme);

                    var emailVm = new EmailVm()
                    {
                        To = registerVm.Email,
                        Subject = "Account Confirmation",
                        Body = $"Dear {user.Email}, please confirm your account by clicking on the following link: <a href=\"" + callBackUrl + "\">click here</a>"
                    };

                    _emailService.SendMail(emailVm);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                AddErrors(result);
            }

            return View(registerVm);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if(userId is null || code is null)
            {
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if(user is null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var viewModel = new LoginVm();
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVm loginVm, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync
                                    (loginVm.Email, loginVm.Password, loginVm.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
                if (result.IsLockedOut)
                {
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "invalid login attempt.");
                    return View(loginVm);
                }
            }

            return View(loginVm);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVm forgotPasswordVm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(forgotPasswordVm.Email);

                if(user == null)
                {
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callBackUrl =
                    Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code },
                    protocol: HttpContext.Request.Scheme);

                var emailVm = new EmailVm()
                {
                    To = forgotPasswordVm.Email,
                    Subject = "Reset Password",
                    Body = $"Dear { user.Email }, please reset your password by clicking on the following link: <a href=\"" + callBackUrl + "\">click here</a>"
                };

                _emailService.SendMail(emailVm);

                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            return View(forgotPasswordVm);
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string code = null)
        {
            //return code == null ? View("Error") : View();

            if(code == null)
            {
                return View("Error");
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVm resetPasswordVm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordVm.Email);

                if (user == null)
                {
                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                }

                var result = await _userManager.ResetPasswordAsync(user, resetPasswordVm.Code, resetPasswordVm.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                }

                AddErrors(result);
            }

            return View();
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(controllerName: "Home", actionName: "Index");
        }
    }
}
