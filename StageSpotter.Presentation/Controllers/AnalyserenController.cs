using Microsoft.AspNetCore.Mvc;
using StageSpotter.Business.Interfaces; 
using StageSpotter.Domain.Models;
using StageSpotter.Web.Models; 

namespace StageSpotter.Web.Controllers
{
    public class AnalyserenController : Controller
    {
        private readonly ICVAnalyseService _cvService;

        public AnalyserenController(ICVAnalyseService cvService)
        {
            _cvService = cvService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
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

            var resultaatViewModel = new CvResultaatViewModel
            {
                BestandsNaam = domainModel.BestandsNaam,
                AnalyseResultaat = domainModel.AiAnalyse ?? "Geen resultaat ontvangen."
            };

            return View("Resultaat", resultaatViewModel);
        }
    }
}
