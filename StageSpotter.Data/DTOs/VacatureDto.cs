namespace StageSpotter.Data.DTOs;

public class VacatureDto
{
    public int Id { get; set; }
    public string Titel { get; set; }
    public string Beschrijving { get; set; }
    public string Locatie { get; set; }
    public string VacatureUrl { get; set; }
    public DateTime PublicatieDatum { get; set; }
    public bool IsActief { get; set; }
    public int SoortStageId { get; set; } 
    
    public BedrijfDto Bedrijf { get; set; } 
    public List<OpleidingsniveauDto> Opleidingsniveaus { get; set; } = new List<OpleidingsniveauDto>();
    public List<StudierichtingDto> Studierichtingen { get; set; } = new List<StudierichtingDto>();
}