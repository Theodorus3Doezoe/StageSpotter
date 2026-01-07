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

    public VacatureController(IVacatureService vacatureService) 
    {
        _vacatureService = vacatureService;
    }
    
    [HttpGet]
    public IActionResult Create()
    {
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
        if (!ModelState.IsValid)
        {
            var alleNiveaus = _vacatureService.GetAlleOpleidingsniveaus();
            var alleRichtingen = _vacatureService.GetAlleStudierichtingen();
            viewModel.OpleidingsniveausList = new SelectList(alleNiveaus, "Id", "Niveau");
            viewModel.StudierichtingenList = new SelectList(alleRichtingen, "Id", "Richting");
                
            return View(viewModel);
        }

        Vacature vacatureModel = VacatureMapper.ToModel(viewModel);

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
}