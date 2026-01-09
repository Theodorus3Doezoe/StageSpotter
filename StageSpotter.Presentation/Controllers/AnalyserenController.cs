using Microsoft.AspNetCore.Mvc;
using StageSpotter.Business.Interfaces; 
using StageSpotter.Domain.Models;
using StageSpotter.Web.Models; 
using System.Security.Claims;

namespace StageSpotter.Web.Controllers
{
    public class AnalyserenController : Controller
    {
        private readonly ICVAnalyseService _cvService;
        private readonly ISavedItemService _savedItemService;

        public AnalyserenController(ICVAnalyseService cvService, ISavedItemService savedItemService)
        {
            _cvService = cvService;
            _savedItemService = savedItemService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var userId = 0;
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                              ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out userId))
            {
                var analyses = _savedItemService.GetAnalysesForUser(userId);
                ViewData["SavedAnalyses"] = analyses;
            }

            return View();
        }

        [HttpPost]
        public IActionResult SaveToProfile()
        {
            var userId = 0;
            
            // Probeer eerst met NameIdentifier claim
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                // Als die niet bestaat, probeer dan Sub claim
                userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
            }
            
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var fileName = Request.Form["cvBestandsnaam"].ToString();
            var resultaat = Request.Form["resultaat"].ToString();

            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(resultaat))
            {
                return RedirectToAction("Index");
            }

            _savedItemService.SaveAnalysis(userId, fileName, resultaat);

            return RedirectToAction("Index", "Profile");
        }

        [HttpPost]
        public async Task<IActionResult> Upload(CvUploadViewModel viewModel)
        {
            if (viewModel.Bestand == null) return View("Index");

            var domainModel = new CvDomain
            {
                BestandsNaam = viewModel.Bestand.FileName
            };

            using (var stream = viewModel.Bestand.OpenReadStream())
            {
                domainModel.BestandsStream = stream;

                await _cvService.AnalyseerCvAsync(domainModel);
            }

            var resultaatViewModel = new CvResultaatViewModel();
            resultaatViewModel.BestandsNaam = domainModel.BestandsNaam;
            
            if (!string.IsNullOrEmpty(domainModel.AiAnalyse))
            {
                resultaatViewModel.AnalyseResultaat = domainModel.AiAnalyse;
            }
            else
            {
                resultaatViewModel.AnalyseResultaat = "Geen resultaat ontvangen.";
            }

            return View("Resultaat", resultaatViewModel);
        }

        [HttpGet]
        public IActionResult LoadSaved(int id)
        {
            var saved = _savedItemService.GetAnalysisById(id);
            if (saved == null)
            {
                return NotFound();
            }

            // Probeer eerst met NameIdentifier claim
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                // Als die niet bestaat, probeer dan Sub claim
                userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
            }
            
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId) || saved.UserId != userId)
            {
                // don't expose others' analyses
                return Forbid();
            }

            var vm = new CvResultaatViewModel
            {
                BestandsNaam = saved.FileName,
                AnalyseResultaat = saved.Result
            };

            return View("Resultaat", vm);
        }
    }
}
