using System.ComponentModel.DataAnnotations;
namespace StageSpotter.Web.Models;

public class EditVacatureViewModel
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "Titel is verplicht")]
    public string Titel { get; set; }

    [Required(ErrorMessage = "Beschrijving is verplicht")]
    public string Beschrijving { get; set; }

    [Required(ErrorMessage = "Locatie is verplicht")]
    public string Locatie { get; set; }

    [Required(ErrorMessage = "Soort stage is verplicht")]
    public int SoortStageId { get; set; }

    [Url(ErrorMessage = "VacatureUrl moet een geldige URL zijn")]
    public string? VacatureUrl { get; set; }
}