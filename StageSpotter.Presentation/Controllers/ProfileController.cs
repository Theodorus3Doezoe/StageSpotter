using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using StageSpotter.Business.Interfaces;

namespace StageSpotter.Presentation.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ISavedItemService _savedItemService;
        private readonly StageSpotter.Data.Interfaces.IVacatureRepository _vacatureRepo;
        private readonly StageSpotter.Data.Interfaces.ISavedMotivationLetterRepository _motivationLetterRepo;

            public ProfileController(ISavedItemService savedItemService, StageSpotter.Data.Interfaces.IVacatureRepository vacatureRepo, StageSpotter.Data.Interfaces.ISavedMotivationLetterRepository motivationLetterRepo)
            {
                _savedItemService = savedItemService;
                _vacatureRepo = vacatureRepo;
                _motivationLetterRepo = motivationLetterRepo;
            }

        public IActionResult Index()
        {
            var email = string.Empty;
            var claim = User.FindFirst(ClaimTypes.Email);
            if (claim != null)
            {
                email = claim.Value;
            }
            else
            {
                var jwtEmail = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email);
                if (jwtEmail != null)
                {
                    email = jwtEmail.Value;
                }
            }

            ViewData["Email"] = email;

            var userId = 0;
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                              ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out userId))
            {
                var analyses = _savedItemService.GetAnalysesForUser(userId);
                var vacatures = _savedItemService.GetVacaturesForUser(userId);
                ViewData["SavedAnalyses"] = analyses;

                var savedItems = new System.Collections.Generic.List<StageSpotter.Presentation.Models.SavedVacatureViewModel>();
                foreach (var sv in vacatures)
                {
                    var dto = _vacatureRepo.GetById(sv.VacatureId);
                    if (dto != null)
                    {
                        var model = new StageSpotter.Presentation.Models.SavedVacatureViewModel();
                        model.VacatureId = dto.Id;
                        
                        if (!string.IsNullOrEmpty(dto.Titel))
                        {
                            model.Titel = dto.Titel;
                        }
                        else
                        {
                            model.Titel = string.Empty;
                        }
                        
                        if (!string.IsNullOrEmpty(dto.Locatie))
                        {
                            model.Locatie = dto.Locatie;
                        }
                        else
                        {
                            model.Locatie = string.Empty;
                        }
                        
                        if (dto.Bedrijf != null && !string.IsNullOrEmpty(dto.Bedrijf.Naam))
                        {
                            model.BedrijfNaam = dto.Bedrijf.Naam;
                        }
                        else
                        {
                            model.BedrijfNaam = string.Empty;
                        }
                        
                        model.SoortStage = ((StageSpotter.Domain.Enums.SoortStage)dto.SoortStageId).ToString();
                        model.CreatedAt = sv.CreatedAt;
                        
                        savedItems.Add(model);
                    }
                    else
                    {
                        var deletedModel = new StageSpotter.Presentation.Models.SavedVacatureViewModel();
                        deletedModel.VacatureId = sv.VacatureId;
                        deletedModel.Titel = "(Vacature verwijderd)";
                        deletedModel.CreatedAt = sv.CreatedAt;
                        savedItems.Add(deletedModel);
                    }
                }

                ViewData["SavedVacatureItems"] = savedItems;

                var motivationLetters = _motivationLetterRepo.GetByUserId(userId);
                ViewData["SavedMotivationLetters"] = motivationLetters;
            }

            return View();
        }

        [HttpPost]
        public IActionResult UnsaveVacature(int vacatureId)
        {
            var userId = 0;
            
            // Probeer eerst met NameIdentifier claim
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                // Als die niet bestaat, probeer dan Sub claim
                userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
            }
            
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out userId))
            {
                _savedItemService.RemoveVacature(userId, vacatureId);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ViewAnalysis(int id)
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

            var sa = _savedItemService.GetAnalysisById(id);
            if (sa == null || sa.UserId != userId)
            {
                return NotFound();
            }

            var vm = new StageSpotter.Web.Models.CvResultaatViewModel
            {
                BestandsNaam = sa.FileName,
                AnalyseResultaat = sa.Result
            };

            return View("~/Views/Analyseren/Resultaat.cshtml", vm);
        }

        [HttpPost]
        public IActionResult DeleteMotivationLetter(int id)
        {
            var userId = 0;
            
            // Probeer eerst met NameIdentifier claim
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                // Als die niet bestaat, probeer dan Sub claim
                userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
            }
            
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out userId))
            {
                _motivationLetterRepo.DeleteById(id);
            }

            return RedirectToAction("Index");
        }
    }
}
