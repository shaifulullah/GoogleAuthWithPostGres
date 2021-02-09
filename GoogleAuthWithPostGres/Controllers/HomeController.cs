using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using GoogleAuthWithPostGres.Data;
using GoogleAuthWithPostGres.Models;
using GoogleAuthWithPostGres.Services;
using GoogleAuthWithPostGres.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleAuthWithPostGres.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private GoogleAuthWithPostGresContext _context;
        private IUser userRepository;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
        public HomeController(GoogleAuthWithPostGresContext context, IUser user)
        {
            _context = context;
            userRepository = user;
        }
        public IActionResult SignIn(bool loggingOut = false, bool initialLogin = true)
        {
            if (loggingOut)
            {
                return View();
            }
            if (!User.Identity.IsAuthenticated && initialLogin)
            {
                return RedirectToAction("SignIn", "Authentication");
            }
            return RedirectToAction("Login", "Home");
        }
        public IActionResult Login()
        {
            //return RedirectToAction("Privacy", "Home");
            try
            {
                bool isGeotab = false;
                string loggedUserName = GetLoggedInUsername();
                string loggedUserEmail = GetLoggedInUserEmail();
                ViewModelUserLogin viewModel = new ViewModelUserLogin() { Email = loggedUserEmail, Name = loggedUserName };

                User found = userRepository.GetUserFromEmail(loggedUserEmail);
                if (found != null)
                {
                    HttpContext.Session.SetObjectAsJson("User", found);
                    //notificationSenderRepository.SendWelcomeEmail(loggedUserEmail, loggedUserName);
                    return RedirectToAction("Privacy", "Home");
                }
                else
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        string geotabDomain = "gmail.com";
                        string userEmail = (User.Identity as System.Security.Claims.ClaimsIdentity)?.Claims.ElementAt(2).Value;
                        string[] x = userEmail.Split("@");
                        string userEmailDomain = x[1];
                        isGeotab = userEmailDomain == geotabDomain;
                    }
                    if (isGeotab)
                    {
                        User newUser = new User() 
                        { 
                            Name = loggedUserName, 
                            Email = loggedUserEmail, 
                            isActive = true,
                            UserTypeInt = UserType.ReadOnly,
                            LoggedInDateTime = DateTime.Now
                        };

                        userRepository.Add(newUser);
                        _context.SaveChanges();

                        //var userExistsInWelcomeEmail = _context.WelcomeEmailTrackers.Where(e => e.NotifiedToEmail.ToLower() == loggedUserEmail.ToLower()).Any();

                        //if (!userExistsInWelcomeEmail)
                        //{
                        //    var welcome = notificationSenderRepository.AddUserEmailToWelcomeEmail(newUser.Email.ToLower(), "myeco@geotab.com");

                        //    _context.Add(welcome);
                        //    _context.SaveChangesAsync();

                        //    notificationSenderRepository.SendWelcomeEmail(newUser.Email, newUser.Name);
                        //}
                        HttpContext.Session.SetObjectAsJson("User", newUser);
                    }
                    return RedirectToAction("Privacy", "Home");
                }
            }
            catch (Exception ex)
            {
                //ViewModelError vm = SetViewModelError(ex);
                //return RedirectToAction("ErrorPage", "ErrorHandler", vm);
                return RedirectToAction("Privacy", "Home");
            }
        }

        //[HttpPost]
        //public IActionResult Login([Bind("Name,Email")] User user)
        //{
        //    try
        //    {
        //        if (user.Name != null && user.Email != null)
        //        {
        //            User found = _context.Users.Where(u => u.Email.Equals(user.Email) && u.Name.Equals(user.Name)).FirstOrDefault();
        //            if (found != null)
        //            {
        //                HttpContext.Session.SetObjectAsJson("User", found);
        //                return RedirectToAction("Index", "Home");
        //            }
        //            else
        //            {
        //                ViewData["Result"] = "Error logging in. No user found.";
        //                ModelState.AddModelError("ErrorMessage", "No user found with the credentials entered.");
        //                return View("Login");
        //            }
        //        }
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewModelError vm = SetViewModelError(ex);
        //        return RedirectToAction("ErrorPage", "ErrorHandler", vm);
        //    }
        //}

        private string GetLoggedInUserEmail()
        {
            string userEmail = "";
            if (User.Identity.IsAuthenticated)
            {
                userEmail = (User.Identity as System.Security.Claims.ClaimsIdentity)?.Claims.ElementAt(2).Value;
            }
            return userEmail;
        }

        private string GetLoggedInUsername()
        {
            string name = "";
            if (User.Identity.IsAuthenticated)
            {
                name = (User.Identity as System.Security.Claims.ClaimsIdentity)?.Claims.ElementAt(1).Value;
            }
            return name;
        }

        public IActionResult Index()
        {
            return View();
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

    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}