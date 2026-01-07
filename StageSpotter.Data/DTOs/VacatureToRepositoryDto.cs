namespace StageSpotter.Data.DTOs;

public class VacatureToRepositoryDto
{
    public string Titel { get; set; }
    public string Beschrijving { get; set; }
    public string Locatie { get; set; }
    public string VacatureUrl { get; set; } 
    public DateTime PublicatieDatum { get; set; } 
    public bool IsActief { get; set; } 
    
    public int SoortStageId { get; set; }
    
    public int BedrijfId { get; set; } 
    
    public List<int> OpleidingsniveauIds { get; set; }
    public List<int> StudierichtingIds { get; set; }
}