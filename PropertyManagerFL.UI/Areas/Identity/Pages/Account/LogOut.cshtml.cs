using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropertyManagerFL.UI;
using System.Security.Claims;

namespace Web.Blazor.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly ILogger<App> _logger;
        public LogoutModel(SignInManager<IdentityUser> signInManager,
                           ILogger<App> logger,
                           UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
        }

        // this code shoudn't be executed here (!!!)
        public async Task<IActionResult> OnGet()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return LocalRedirect("/");
        }


        // didn't get called (??)
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
            var userName = User.FindFirstValue(ClaimTypes.Name); // will give the user's userName
            var userEmail = User.FindFirstValue(ClaimTypes.Email);


            await _signInManager.SignOutAsync();
            _logger.LogInformation($"User {userName} logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // Redirect the user to application root
                return LocalRedirect("/");
                //return LocalRedirect("~/Identity/Account/Login");
            }
        }
    }
}


