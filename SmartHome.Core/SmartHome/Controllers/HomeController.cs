using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartHome.Model;
using SmartHome.Models;
using SmartHome.Models.Security;

namespace SmartHome.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly SmartHomeUser _user;

        public HomeController(IHttpContextAccessor httpContextAccessor, UserManager<SmartHomeUser> userManager, IOptions<List<UserProfile>> userOptions)
        {
            var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = userManager.FindByIdAsync(userId).GetAwaiter().GetResult();
            if (user != null)
            {
                user.Profiles = (from p in userOptions.Value
                                from a in user.AvailableProfiles
                                where p.Name.Equals(a)
                                select p)
                                .ToList();

                user.Profiles.ForEach(p => p.Content.ForEach(c => c.UpdateParents()));

                _user = user;
            }
        }

        public IActionResult Index()
        {
            return View(_user);
        }

        [Route("Profile/{profile}")]
        public IActionResult Profile(string profile)
        {
            _user.ActiveProfileName = profile;
            return View("Index", _user);
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
