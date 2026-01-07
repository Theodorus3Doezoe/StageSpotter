using Microsoft.AspNetCore.Mvc;
using StageSpotter.Business.Interfaces;
using StageSpotter.Web.Models; 
using StageSpotter.Domain.Models;
namespace StageSpotter.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVacatureService _vacatureService;
        private const int VACATURES_PER_PAGINA = 5;

        public HomeController(IVacatureService vacatureService)
        {
            _vacatureService = vacatureService;
        }
        public IActionResult Index(string? zoekterm, int pagina = 1, int? id = null)
        {
            var alleVacatures = _vacatureService.GetAlleVacatures();
            
            // Filter vacatures
            var gefilterdeVacatures = alleVacatures;
            if (!string.IsNullOrEmpty(zoekterm))
            {
                gefilterdeVacatures = alleVacatures.Where(v => 
                    v.Titel.Contains(zoekterm, StringComparison.OrdinalIgnoreCase) ||
                    v.Beschrijving.Contains(zoekterm, StringComparison.OrdinalIgnoreCase) ||
                    v.Bedrijf.Naam.Contains(zoekterm, StringComparison.OrdinalIgnoreCase) ||
                    v.Locatie.Contains(zoekterm, StringComparison.OrdinalIgnoreCase) ||
                    v.Opleidingsniveaus.Any(o => o.Niveau.Contains(zoekterm, StringComparison.OrdinalIgnoreCase)) ||
                    v.Studierichtingen.Any(s => s.Richting.Contains(zoekterm, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            // Bereken paginering
            var totaalAantalVacatures = gefilterdeVacatures.Count;
            var totaalAantalPaginas = (int)Math.Ceiling((double)totaalAantalVacatures / VACATURES_PER_PAGINA);
            if (totaalAantalPaginas <= 0) totaalAantalPaginas = 1;
            if (pagina < 1) pagina = 1;
            if (pagina > totaalAantalPaginas) pagina = totaalAantalPaginas;

            var vacatureEntitiesVoorPagina = gefilterdeVacatures
                .Skip((pagina - 1) * VACATURES_PER_PAGINA) // Bereken hoeveel vacatures je moet overslaan voor de juiste pagina
                .Take(VACATURES_PER_PAGINA) // Selecteer de volgende 5 pagina's
                .ToList();

            var viewModel = new VacatureOverzichtViewModel
            {
                Zoekterm = zoekterm,
                HuidigePagina = pagina,
                TotaalPaginas = totaalAantalPaginas,
                TotaalVacatures = totaalAantalVacatures,
                
                // Map elke vacature(v) om in een new VacatureLijstItem
                Vacatures = vacatureEntitiesVoorPagina.Select(v => new VacatureLijstItem
                {
                    Id = v.Id,
                    Titel = v.Titel,
                    BedrijfNaam = v.Bedrijf?.Naam ?? "Onbekend", // Null-check!
                    Locatie = v.Locatie,
                    PublicatieDatum = v.PublicatieDatum,
                    Opleidingen = string.Join(" - ", v.Opleidingsniveaus.Select(o => o.Niveau)),
                    Studierichtingen = string.Join(" - ", v.Studierichtingen.Select(s => s.Richting))
                }).ToList()
            };

            // Logica voor gesecteerde vacature
            Vacature? geselecteerdeEntity = null;
            
            if (id.HasValue) // Check voor een id in de url
            {   // Zoek in de lijst naar de vacature met het id in de url
                geselecteerdeEntity = gefilterdeVacatures.FirstOrDefault(v => v.Id == id.Value);
            }
            else
            {   // Anders selecteer de eerste vacature
                geselecteerdeEntity = vacatureEntitiesVoorPagina.FirstOrDefault();
            }

            if (geselecteerdeEntity != null)
            {
                viewModel.GeselecteerdeVacature = new VacatureDetailItem
                {
                    Id = geselecteerdeEntity.Id,
                    Titel = geselecteerdeEntity.Titel,
                    BedrijfNaam = geselecteerdeEntity.Bedrijf?.Naam ?? "Onbekend",
                    Locatie = geselecteerdeEntity.Locatie,
                    SoortStage = geselecteerdeEntity.SoortStage.ToString(),
                    Beschrijving = geselecteerdeEntity.Beschrijving,
                    Opleidingen = string.Join(" - ", geselecteerdeEntity.Opleidingsniveaus.Select(o => o.Niveau)),
                    Studierichtingen = string.Join(" - ", geselecteerdeEntity.Studierichtingen.Select(s => s.Richting)),
                    VacatureUrl = geselecteerdeEntity.VacatureUrl
                };
            }
            
            return View(viewModel);
        }
    }
}