using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UMLProjectX.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using UMLProjectX.DAL;
using UMLProjectX.DAL.Models;

namespace UMLProjectX.Controllers
{
    public class AccountController : Controller
    {
        private DataContext _db;

        public AccountController(DataContext context)
        {
            _db = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _db.UserValid(model.Login, model.Password);
                if (user != null)
                {
                    await Authenticate(user);

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                if (!_db.ContainsLogin(model.Login))
                {
                    await Authenticate(_db.AddUser(new User { Login = model.Login, Password = model.Password, Role = "default" }));

                    return RedirectToAction("Index", "Home");
                }
                
                ModelState.AddModelError("", "Логин уже используется");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Profile(string login)
        {
            var user = _db.FindUserByLogin(login);
            var reviews = _db.FindReviewsForUser(user.UserId);

            ViewBag.Login = user.Login;
            ViewBag.Reviews = reviews;

            return View();
        }

        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
