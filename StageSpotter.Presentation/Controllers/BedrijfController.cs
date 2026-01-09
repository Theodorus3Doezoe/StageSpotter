using Microsoft.AspNetCore.Mvc;
using StageSpotter.Business.Interfaces;
using StageSpotter.Data.Interfaces;
using StageSpotter.Domain.Models;
using System.Security.Claims;

namespace StageSpotter.Presentation.Controllers
{
    public class BedrijfController : Controller
    {
        private readonly IBedrijfRepository _bedrijfRepo;
        private readonly IReviewService _reviewService;

        public BedrijfController(IBedrijfRepository bedrijfRepo, IReviewService reviewService)
        {
            _bedrijfRepo = bedrijfRepo;
            _reviewService = reviewService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var companies = _bedrijfRepo.GetAll();
            return View(companies);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var bedrijf = _bedrijfRepo.GetById(id);
            if (bedrijf == null) return NotFound();

            var reviews = _reviewService.GetReviewsForBedrijf(bedrijf.Id);
            var avg = _reviewService.GetAverageRating(bedrijf.Id);

            ViewData["Company"] = bedrijf;
            ViewData["Reviews"] = reviews;
            ViewData["AverageRating"] = avg;

            return View();
        }

        [HttpPost]
        public IActionResult AddReview(int bedrijfId, string title, string description, int rating)
        {
        // Probeer eerst met NameIdentifier claim
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            // Als die niet bestaat, probeer dan Sub claim
            userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
        }
        
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (rating < 1) rating = 1;
            if (rating > 5) rating = 5;

var review = new Review();
        review.UserId = userId;
        review.BedrijfId = bedrijfId;
        
        if (!string.IsNullOrEmpty(title))
        {
            review.Title = title;
        }
        else
        {
            review.Title = string.Empty;
        }
        
        review.Description = description;
        review.Rating = rating;
        review.CreatedAt = System.DateTime.UtcNow;

            _reviewService.AddReview(review);

            return RedirectToAction("Details", new { id = bedrijfId });
        }
    }
}
