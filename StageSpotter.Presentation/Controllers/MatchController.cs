using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using StageSpotter.Business.Interfaces;

namespace StageSpotter.Presentation.Controllers
{
    public class MatchController : Controller
    {
        private readonly IMatchService _matchService;

        public MatchController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Probeer eerst met NameIdentifier claim
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                // Als die niet bestaat, probeer dan Sub claim
                userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
            }
            
            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            
            var userId = int.Parse(userIdClaim.Value);
            var matches = _matchService.GetMatchesForUser(userId);
            return View(matches);
        }
    }
}
