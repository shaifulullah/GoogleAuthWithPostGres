using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleAuthWithPostGres.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;
        public AuthenticationController(IAuthenticationSchemeProvider _IAuthenticationSchemeProvider)
        {
            _authenticationSchemeProvider = _IAuthenticationSchemeProvider;
        }
        public async Task<IActionResult> Login()
        {
            // this is the view
            var allSchemeProvider = (await _authenticationSchemeProvider.GetAllSchemesAsync())
                .Select(n => n.DisplayName).Where(n => !string.IsNullOrEmpty(n));
            return View(allSchemeProvider);
        }
        public IActionResult SignIn(string provider = "Google")
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, provider);
        }

        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Authentication");
        }
    }
}