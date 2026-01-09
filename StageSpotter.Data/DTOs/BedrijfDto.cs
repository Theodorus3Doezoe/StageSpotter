namespace StageSpotter.Data.DTOs;

public class BedrijfDto
{
    public int Id { get; set; }
    public string Naam { get; set; }
    public string BedrijfUrl { get; set; }
    public string? KvKNummer { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactEmail { get; set; }
}