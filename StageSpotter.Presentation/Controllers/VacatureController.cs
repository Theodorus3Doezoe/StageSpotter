using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StageSpotter.Web.Models;          
using StageSpotter.Business.Interfaces; 
using StageSpotter.Web.Mappers;         
using StageSpotter.Domain.Models;
  

namespace StageSpotter.Web.Controllers;

public class VacatureController : Controller
{
    private readonly IVacatureService _vacatureService; 
    private readonly StageSpotter.Data.Interfaces.IBedrijfRepository _bedrijfRepository;
    private readonly ISavedItemService _savedItemService;
    private readonly IMotivationLetterService _motivationLetterService;
    private readonly StageSpotter.Data.Interfaces.ISavedMotivationLetterRepository _motivationLetterRepository;

    public VacatureController(IVacatureService vacatureService, ISavedItemService savedItemService, IMotivationLetterService motivationLetterService, StageSpotter.Data.Interfaces.ISavedMotivationLetterRepository motivationLetterRepository, StageSpotter.Data.Interfaces.IBedrijfRepository bedrijfRepository)
    {
        _vacatureService = vacatureService;
        _savedItemService = savedItemService;
        _motivationLetterService = motivationLetterService;
        _motivationLetterRepository = motivationLetterRepository;
        _bedrijfRepository = bedrijfRepository;
    }
    
    [HttpGet]
    public IActionResult Create()
    {
        var userTypeClaim = User.FindFirst("usertype");
        if (userTypeClaim == null || userTypeClaim.Value != "bedrijf")
        {
            return Forbid();
        }
        var viewModel = new CreateVacatureViewModel();

        var alleNiveaus = _vacatureService.GetAlleOpleidingsniveaus();
        var alleRichtingen = _vacatureService.GetAlleStudierichtingen();

        viewModel.OpleidingsniveausList = new SelectList(alleNiveaus, "Id", "Niveau");
        viewModel.StudierichtingenList = new SelectList(alleRichtingen, "Id", "Richting");

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult Create(CreateVacatureViewModel viewModel)
    {
        var userTypeClaim = User.FindFirst("usertype");
        if (userTypeClaim == null || userTypeClaim.Value != "bedrijf")
        {
            return Forbid();
        }
        if (!ModelState.IsValid)
        {
            var alleNiveaus = _vacatureService.GetAlleOpleidingsniveaus();
            var alleRichtingen = _vacatureService.GetAlleStudierichtingen();
            viewModel.OpleidingsniveausList = new SelectList(alleNiveaus, "Id", "Niveau");
            viewModel.StudierichtingenList = new SelectList(alleRichtingen, "Id", "Richting");
                
            return View(viewModel);
        }

        Vacature vacatureModel = VacatureMapper.ToModel(viewModel);
        // Koppel vacature aan het bedrijf van de gebruiker
        var bedrijfIdClaim = User.FindFirst("bedrijfId");
        if (bedrijfIdClaim != null && int.TryParse(bedrijfIdClaim.Value, out var bedrijfId))
        {
            var bedrijfDto = _bedrijfRepository.GetById(bedrijfId);
            if (bedrijfDto != null)
            {
                vacatureModel.Bedrijf = new Bedrijf { Naam = bedrijfDto.Naam };
            }
        }

        try
        {
            _vacatureService.CreateVacature(vacatureModel);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            
            var alleNiveaus = _vacatureService.GetAlleOpleidingsniveaus();
            var alleRichtingen = _vacatureService.GetAlleStudierichtingen();
            viewModel.OpleidingsniveausList = new SelectList(alleNiveaus, "Id", "Niveau");
            viewModel.StudierichtingenList = new SelectList(alleRichtingen, "Id", "Richting");
                
            return View(viewModel);
        }
            
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Mijn()
    {
        var userTypeClaim = User.FindFirst("usertype");
        if (userTypeClaim == null || userTypeClaim.Value != "bedrijf")
        {
            return Forbid();
        }
        var bedrijfIdClaim = User.FindFirst("bedrijfId");
        if (bedrijfIdClaim == null || !int.TryParse(bedrijfIdClaim.Value, out var bedrijfId))
        {
            return Forbid();
        }
        var all = _vacatureService.GetAlleVacatures();
        
        // Filter vacatures van dit bedrijf
        var mine = new List<Vacature>();
        foreach (var v in all)
        {
            bool isMijn = false;
            if (v.Bedrijf != null && v.Bedrijf.Id == bedrijfId)
            {
                isMijn = true;
            }
            else if (v.BedrijfId == bedrijfId)
            {
                isMijn = true;
            }
            
            if (isMijn)
            {
                mine.Add(v);
            }
        }
        
        // Maak een lijst met view models
        var items = new List<StageSpotter.Web.Models.VacatureLijstItem>();
        foreach (var v in mine)
        {
            var item = new StageSpotter.Web.Models.VacatureLijstItem();
            item.Id = v.Id;
            item.Titel = v.Titel;
            if (v.Bedrijf != null && !string.IsNullOrEmpty(v.Bedrijf.Naam))
            {
                item.BedrijfNaam = v.Bedrijf.Naam;
            }
            else
            {
                item.BedrijfNaam = "";
            }
            item.Locatie = v.Locatie;
            
            // Bouw opleidingen string
            string[] opleidingNamen = new string[v.Opleidingsniveaus.Count];
            for (int i = 0; i < v.Opleidingsniveaus.Count; i++)
            {
                opleidingNamen[i] = v.Opleidingsniveaus[i].Niveau;
            }
            item.Opleidingen = string.Join(" - ", opleidingNamen);
            
            // Bouw studierichtingen string
            string[] richtingNamen = new string[v.Studierichtingen.Count];
            for (int i = 0; i < v.Studierichtingen.Count; i++)
            {
                richtingNamen[i] = v.Studierichtingen[i].Richting;
            }
            item.Studierichtingen = string.Join(" - ", richtingNamen);
            
            item.PublicatieDatum = v.PublicatieDatum;
            items.Add(item);
        }
        return View(items);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var userTypeClaim = User.FindFirst("usertype");
        if (userTypeClaim == null || userTypeClaim.Value != "bedrijf")
        {
            return Forbid();
        }
        var bedrijfIdClaim = User.FindFirst("bedrijfId");
        if (bedrijfIdClaim == null || !int.TryParse(bedrijfIdClaim.Value, out var bedrijfId))
        {
            return Forbid();
        }

        var vacature = _vacatureService.GetVacatureById(id);
        if (vacature == null || (vacature.BedrijfId != bedrijfId && vacature.Bedrijf?.Id != bedrijfId))
        {
            return NotFound();
        }

        var vm = new StageSpotter.Web.Models.EditVacatureViewModel
        {
            Id = vacature.Id,
            Titel = vacature.Titel,
            Beschrijving = vacature.Beschrijving,
            Locatie = vacature.Locatie,
            SoortStageId = (int)vacature.SoortStage,
            VacatureUrl = vacature.VacatureUrl
        };
        return View(vm);
    }

    [HttpPost]
    public IActionResult Edit(StageSpotter.Web.Models.EditVacatureViewModel vm)
    {
        var userTypeClaim = User.FindFirst("usertype");
        if (userTypeClaim == null || userTypeClaim.Value != "bedrijf")
        {
            return Forbid();
        }
        var bedrijfIdClaim = User.FindFirst("bedrijfId");
        if (bedrijfIdClaim == null || !int.TryParse(bedrijfIdClaim.Value, out var bedrijfId))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var existing = _vacatureService.GetVacatureById(vm.Id);
        if (existing == null || (existing.BedrijfId != bedrijfId && existing.Bedrijf?.Id != bedrijfId))
        {
            return NotFound();
        }

        existing.Titel = vm.Titel;
        existing.Beschrijving = vm.Beschrijving;
        existing.Locatie = vm.Locatie;
        existing.SoortStage = (StageSpotter.Domain.Enums.SoortStage)vm.SoortStageId;
        existing.VacatureUrl = vm.VacatureUrl ?? existing.VacatureUrl;

        var ok = _vacatureService.UpdateVacature(existing, bedrijfId);
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, "Kon vacature niet bijwerken.");
            return View(vm);
        }
        return RedirectToAction("Mijn");
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var userTypeClaim = User.FindFirst("usertype");
        if (userTypeClaim == null || userTypeClaim.Value != "bedrijf")
        {
            return Forbid();
        }
        var bedrijfIdClaim = User.FindFirst("bedrijfId");
        if (bedrijfIdClaim == null || !int.TryParse(bedrijfIdClaim.Value, out var bedrijfId))
        {
            return Forbid();
        }

        var ok = _vacatureService.DeactivateVacature(id, bedrijfId);
        if (!ok)
        {
            TempData["Error"] = "Kon vacature niet verwijderen.";
        }
        return RedirectToAction("Mijn");
    }

    [HttpPost]
    public IActionResult SaveToProfile(int vacatureId)
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
        _savedItemService?.SaveVacature(userId, vacatureId);
        return RedirectToAction("Index", "Profile");
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        var all = _vacatureService.GetAlleVacatures();
        
        // Zoek de vacature met het gegeven id
        Vacature v = null;
        foreach (var vacature in all)
        {
            if (vacature.Id == id)
            {
                v = vacature;
                break;
            }
        }
        
        if (v == null)
        {
            return NotFound();
        }

        var model = new StageSpotter.Web.Models.VacatureDetailItem
        {
            Id = v.Id,
            Titel = v.Titel,
            BedrijfNaam = v.Bedrijf?.Naam ?? "Onbekend",
            BedrijfId = v.Bedrijf?.Id ?? v.BedrijfId,
            Locatie = v.Locatie,
            SoortStage = v.SoortStage.ToString(),
            Beschrijving = v.Beschrijving,
            VacatureUrl = v.VacatureUrl,
            Opleidingen = string.Join(" - ", v.Opleidingsniveaus.Select(o => o.Niveau)),
            Studierichtingen = string.Join(" - ", v.Studierichtingen.Select(s => s.Richting))
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> GenerateMotivationLetter(int vacatureId)
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

        try
        {
            var letter = await _motivationLetterService.GenerateMotivationLetterAsync(userId, vacatureId);
            return Json(new { success = true, letter = letter });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult SaveMotivationLetter(int vacatureId, string content, string vacatureTitel, string bedrijfNaam)
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
            return Json(new { success = false, message = "Niet ingelogd" });
        }

        try
        {
            _motivationLetterRepository.SaveMotivationLetter(userId, vacatureId, content, vacatureTitel, bedrijfNaam);
            return Json(new { success = true, message = "Brief opgeslagen!" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}