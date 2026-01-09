using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using StageSpotter.Business.Interfaces;
using StageSpotter.Data.DTOs;

namespace StageSpotter.Presentation.Controllers
{
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Register(RegisterDto model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return View(model);
            }

            int id;
            if (model.IsBedrijf)
            {
                var bedrijfDto = new StageSpotter.Data.DTOs.BedrijfDto
                {
                    Naam = model.Bedrijfsnaam ?? string.Empty,
                    BedrijfUrl = model.BedrijfUrl ?? string.Empty,
                    KvKNummer = model.KvKNummer,
                    ContactPerson = model.ContactPerson,
                    ContactEmail = model.ContactEmail
                };
                id = await _authService.RegisterAsync(model.Email, model.Password, StageSpotter.Domain.Models.UserType.Bedrijf, bedrijfDto);
            }
            else
            {
                id = await _authService.RegisterAsync(model.Email, model.Password);
            }
            if (id == 0)
            {
                ModelState.AddModelError(string.Empty, "Email already in use");
                return View(model);
            }

            var token = await _authService.LoginAsync(model.Email, model.Password);
            if (!string.IsNullOrEmpty(token))
            {
                var options = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    Expires = System.DateTimeOffset.UtcNow.AddHours(1)
                };
                Response.Cookies.Append("AuthToken", token, options);
            }

            return RedirectToAction("Index", "Quiz");
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Login(LoginDto model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return View(model);
            }

            var token = await _authService.LoginAsync(model.Email, model.Password);
            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError(string.Empty, "Invalid credentials");
                return View(model);
            }

            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps, // send secure only when using HTTPS
                SameSite = SameSiteMode.Lax,
                Expires = System.DateTimeOffset.UtcNow.AddHours(1)
            };
            Response.Cookies.Append("AuthToken", token, options);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            return RedirectToAction("Index", "Home");
        }
    }
}
