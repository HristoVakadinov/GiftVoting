using Microsoft.AspNetCore.Mvc;
using Gifts.Services.Interfaces.Authentication;
using Gifts.Web.Models.ViewModels.Account;
using Gifts.Services.DTOs.Authentication;

namespace Gifts.Web.Controllers.Authentication
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationService _authenticationService;

        public AccountController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var result = await _authenticationService.LoginAsync(new LoginRequest
            {
                Username = request.Username,
                Password = request.Password
            }); 

            if (result.Success)
            {
                HttpContext.Session.SetInt32("UserId", result.UserId.Value);
                HttpContext.Session.SetString("Username", result.FullName);

                if (!string.IsNullOrEmpty(request.ReturnUrl))
                {
                    return Redirect(request.ReturnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            ViewData["ErrorMessage"] = result.Message ?? "Invalid username or password";
            return View(request);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}