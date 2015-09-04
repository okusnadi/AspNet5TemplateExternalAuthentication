using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Authentication.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!string.IsNullOrWhiteSpace(userName) && 
                userName == password)
            {
                var claims = new List<Claim>
                    {
                        new Claim("sub", userName),
                        new Claim("name", "Bob"),
                        new Claim("email", "bob@smith.com")
                    };

                var id = new ClaimsIdentity(claims, "local", "name", "role");
                await Context.Authentication.SignInAsync("Cookies", new ClaimsPrincipal(id));

                return RedirectToLocal(returnUrl);
            }

            return View();
        }

        public IActionResult External(string provider)
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = "/home/secure"
            };

            return new ChallengeResult(provider, props);
        }

        public async Task<IActionResult> Logoff()
        {
            await Context.Authentication.SignOutAsync("Cookies");
            return RedirectToAction(nameof(HomeController.Index), "Home");
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
    }
}