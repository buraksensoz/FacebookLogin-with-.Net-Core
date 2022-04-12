using FacebookLogin.App.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FacebookLogin.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IHttpContextAccessor _httpContextAccessor;
        public HomeController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task LoginFacebook()
        {
            //Redirect  "RedirectUri"  After the Login FaceBook

            await HttpContext.ChallengeAsync(FacebookDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = new PathString("/Home/LoggedUserInfo")
            });
        }

        public IActionResult LoggedUserInfo()
        {

            //Get Facebook Data From Cookie
            var userrec = _httpContextAccessor.HttpContext.User;
            var model = new UserModel()
            {
                Email = userrec.FindFirstValue(ClaimTypes.Email),
                UserName = userrec.FindFirstValue(ClaimTypes.Name),
                Identifier = userrec.FindFirstValue(ClaimTypes.NameIdentifier),
                Born = userrec.FindFirstValue(ClaimTypes.DateOfBirth),
                PhotoUrl = _httpContextAccessor.HttpContext.User.FindFirstValue("pictureUrl")
            };

            return View(model);
        }

        public async Task<IActionResult> SignOut()
        {
            //Signout FaceLogin..
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("index");
        }

        public IActionResult Privacy()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
