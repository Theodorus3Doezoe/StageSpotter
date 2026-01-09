using Microsoft.AspNetCore.Mvc;
using StageSpotter.Business.Interfaces;
using StageSpotter.Web.Models; 
using StageSpotter.Domain.Models;
namespace StageSpotter.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IVacatureService _vacatureService;
        private readonly StageSpotter.Business.Interfaces.IReviewService _reviewService;
        private const int VACATURES_PER_PAGINA = 5;

        public HomeController(IVacatureService vacatureService, StageSpotter.Business.Interfaces.IReviewService reviewService)
        {
            _vacatureService = vacatureService;
            _reviewService = reviewService;
        }
        public IActionResult Index(string? zoekterm, int pagina = 1, int? id = null)
        {
            var alleVacatures = _vacatureService.GetAlleVacatures();
            
            // Filter vacatures op basis van zoekterm
            var gefilterdeVacatures = alleVacatures;
            if (!string.IsNullOrEmpty(zoekterm))
            {
                List<Vacature> resultaat = new List<Vacature>();
                
                foreach (var vacature in alleVacatures)
                {
                    bool matchGevonden = false;
                    
                    // Check of zoekterm voorkomt in titel
                    if (vacature.Titel.Contains(zoekterm, StringComparison.OrdinalIgnoreCase))
                    {
                        matchGevonden = true;
                    }
                    
                    // Check of zoekterm voorkomt in beschrijving
                    if (vacature.Beschrijving.Contains(zoekterm, StringComparison.OrdinalIgnoreCase))
                    {
                        matchGevonden = true;
                    }
                    
                    // Check of zoekterm voorkomt in bedrijfsnaam
                    if (vacature.Bedrijf.Naam.Contains(zoekterm, StringComparison.OrdinalIgnoreCase))
                    {
                        matchGevonden = true;
                    }
                    
                    // Check of zoekterm voorkomt in locatie
                    if (vacature.Locatie.Contains(zoekterm, StringComparison.OrdinalIgnoreCase))
                    {
                        matchGevonden = true;
                    }
                    
                    // Check of zoekterm voorkomt in opleidingsniveaus
                    foreach (var niveau in vacature.Opleidingsniveaus)
                    {
                        if (niveau.Niveau.Contains(zoekterm, StringComparison.OrdinalIgnoreCase))
                        {
                            matchGevonden = true;
                            break;
                        }
                    }
                    
                    // Check of zoekterm voorkomt in studierichtingen
                    foreach (var richting in vacature.Studierichtingen)
                    {
                        if (richting.Richting.Contains(zoekterm, StringComparison.OrdinalIgnoreCase))
                        {
                            matchGevonden = true;
                            break;
                        }
                    }
                    
                    if (matchGevonden)
                    {
                        resultaat.Add(vacature);
                    }
                }
                
                gefilterdeVacatures = resultaat;
            }

            // Bereken paginering
            var totaalAantalVacatures = gefilterdeVacatures.Count;
            var totaalAantalPaginas = (int)Math.Ceiling((double)totaalAantalVacatures / VACATURES_PER_PAGINA);
            if (totaalAantalPaginas <= 0) totaalAantalPaginas = 1;
            if (pagina < 1) pagina = 1;
            if (pagina > totaalAantalPaginas) pagina = totaalAantalPaginas;

            // Bereken welke vacatures op deze pagina moeten komen
            int aantalTeOverslaan = (pagina - 1) * VACATURES_PER_PAGINA;
            List<Vacature> vacatureEntitiesVoorPagina = new List<Vacature>();
            
            for (int i = aantalTeOverslaan; i < gefilterdeVacatures.Count && i < aantalTeOverslaan + VACATURES_PER_PAGINA; i++)
            {
                vacatureEntitiesVoorPagina.Add(gefilterdeVacatures[i]);
            }

            var viewModel = new VacatureOverzichtViewModel
            {
                Zoekterm = zoekterm,
                HuidigePagina = pagina,
                TotaalPaginas = totaalAantalPaginas,
                TotaalVacatures = totaalAantalVacatures,
                
                Vacatures = new List<VacatureLijstItem>()
            };
            
            // Zet elke vacature om naar een VacatureLijstItem voor de view
            foreach (var vacature in vacatureEntitiesVoorPagina)
            {
                var item = new VacatureLijstItem();
                item.Id = vacature.Id;
                item.Titel = vacature.Titel;
                
                // Check of bedrijf bestaat
                if (vacature.Bedrijf != null)
                {
                    item.BedrijfNaam = vacature.Bedrijf.Naam;
                    item.BedrijfId = vacature.Bedrijf.Id;
                }
                else
                {
                    item.BedrijfNaam = "Onbekend";
                    item.BedrijfId = vacature.BedrijfId;
                }
                
                item.Locatie = vacature.Locatie;
                item.PublicatieDatum = vacature.PublicatieDatum;
                
                // Maak een string van alle opleidingsniveaus
                List<string> niveaus = new List<string>();
                foreach (var niveau in vacature.Opleidingsniveaus)
                {
                    niveaus.Add(niveau.Niveau);
                }
                item.Opleidingen = string.Join(" - ", niveaus);
                
                // Maak een string van alle studierichtingen
                List<string> richtingen = new List<string>();
                foreach (var richting in vacature.Studierichtingen)
                {
                    richtingen.Add(richting.Richting);
                }
                item.Studierichtingen = string.Join(" - ", richtingen);
                
                // Bereken gemiddelde rating en rond af op 2 decimalen
                double rating = _reviewService.GetAverageRating(vacature.Id);
                item.AverageRating = Math.Round(rating, 2);
                
                viewModel.Vacatures.Add(item);
            }
            
            viewModel.TotaalPaginas = totaalAantalPaginas;
viewModel.TotaalVacatures = totaalAantalVacatures;           

            // Zoek de geselecteerde vacature
            Vacature geselecteerdeEntity = null;
            
            if (id.HasValue)
            {
                // Er is een id meegegeven in de URL, zoek deze vacature
                foreach (var vacature in gefilterdeVacatures)
                {
                    if (vacature.Id == id.Value)
                    {
                        geselecteerdeEntity = vacature;
                        break;
                    }
                }
            }
            else
            {
                // Geen id meegegeven, kies de eerste vacature van deze pagina
                if (vacatureEntitiesVoorPagina.Count > 0)
                {
                    geselecteerdeEntity = vacatureEntitiesVoorPagina[0];
                }
            }

            // Zet geselecteerde vacature om naar detail model voor de view
            if (geselecteerdeEntity != null)
            {
                var detailItem = new VacatureDetailItem();
                detailItem.Id = geselecteerdeEntity.Id;
                detailItem.Titel = geselecteerdeEntity.Titel;
                
                // Check of bedrijf bestaat
                if (geselecteerdeEntity.Bedrijf != null)
                {
                    detailItem.BedrijfNaam = geselecteerdeEntity.Bedrijf.Naam;
                    detailItem.BedrijfId = geselecteerdeEntity.Bedrijf.Id;
                }
                else
                {
                    detailItem.BedrijfNaam = "Onbekend";
                    detailItem.BedrijfId = geselecteerdeEntity.BedrijfId;
                }
                
                detailItem.Locatie = geselecteerdeEntity.Locatie;
                detailItem.SoortStage = geselecteerdeEntity.SoortStage.ToString();
                detailItem.Beschrijving = geselecteerdeEntity.Beschrijving;
                detailItem.VacatureUrl = geselecteerdeEntity.VacatureUrl;
                
                // Maak string van opleidingsniveaus
                List<string> niveauLijst = new List<string>();
                foreach (var niveau in geselecteerdeEntity.Opleidingsniveaus)
                {
                    niveauLijst.Add(niveau.Niveau);
                }
                detailItem.Opleidingen = string.Join(" - ", niveauLijst);
                
                // Maak string van studierichtingen
                List<string> richtingLijst = new List<string>();
                foreach (var richting in geselecteerdeEntity.Studierichtingen)
                {
                    richtingLijst.Add(richting.Richting);
                }
                detailItem.Studierichtingen = string.Join(" - ", richtingLijst);
                
                viewModel.GeselecteerdeVacature = detailItem;
            }
            
            return View(viewModel);
        }
    }
}