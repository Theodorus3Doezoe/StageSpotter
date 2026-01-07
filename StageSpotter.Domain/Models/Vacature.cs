namespace StageSpotter.Domain.Models;
using StageSpotter.Domain.Enums;

public class Vacature
{
    public int Id { get; set; }
    public string Titel { get; set; }
    public string Beschrijving { get; set; }
    public string Locatie { get; set; }
    public string VacatureUrl { get; set; }
    public DateTime PublicatieDatum { get; set; }
    public bool IsActief { get; set; }
    
    // Enum properties
    public SoortStage SoortStage { get; set; }
    
    // Relaties
    public int BedrijfId { get; set; }
    public Bedrijf Bedrijf { get; set; }
    
    public List<Opleidingsniveau> Opleidingsniveaus { get; set; } = new List<Opleidingsniveau>();
    public List<Studierichting> Studierichtingen { get; set; } = new List<Studierichting>();
}