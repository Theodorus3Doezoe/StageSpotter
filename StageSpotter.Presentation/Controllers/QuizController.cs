using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using StageSpotter.Business.Interfaces;
using StageSpotter.Domain.Models;

namespace StageSpotter.Presentation.Controllers
{
    public class QuizController : Controller
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = int.Parse(userIdClaim.Value);
            var prefs = _quizService.GetPreferencesByUserId(userId);
            return View(prefs);
        }

        [HttpPost]
        public IActionResult Save(UserPreference model)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            model.UserId = int.Parse(userIdClaim.Value);
            _quizService.SavePreferences(model);
            return RedirectToAction("Index", "Home");
        }
    }
}
