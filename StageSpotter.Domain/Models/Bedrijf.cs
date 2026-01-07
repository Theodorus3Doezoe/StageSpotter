namespace StageSpotter.Domain.Models;

public class Bedrijf
{
    public int Id { get; set; }
    public string Naam { get; set; }
    public string BedrijfUrl { get; set; }
    public ICollection<Vacature> Vacatures { get; set; }
}