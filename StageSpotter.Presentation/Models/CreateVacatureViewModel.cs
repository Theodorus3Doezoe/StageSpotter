using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
namespace StageSpotter.Web.Models;

public class CreateVacatureViewModel
{
    [Required(ErrorMessage = "Titel is verplicht")]
    public string Titel { get; set; }

    [Required(ErrorMessage = "Bedrijfsnaam is verplicht")]
    public string Bedrijfnaam { get; set; }

    [Required(ErrorMessage = "Beschrijving is verplicht")]
    public string Beschrijving { get; set; }

    [Required(ErrorMessage = "Locatie is verplicht")]
    public string Locatie { get; set; }

    [Required(ErrorMessage = "Soort stage is verplicht")]
    public int SoortStageId { get; set; }

    [Required(ErrorMessage = "Selecteer minimaal één opleidingsniveau")]
    public List<int> OpleidingsniveauIds { get; set; } = new();

    [Required(ErrorMessage = "Selecteer minimaal één studierichting")]
    public List<int> StudierichtingIds { get; set; } = new();

    [Url(ErrorMessage = "VacatureUrl moet een geldige URL zijn")]
    public string? VacatureUrl { get; set; }

    public SelectList? OpleidingsniveausList { get; set; }
    public SelectList? StudierichtingenList { get; set; }
}