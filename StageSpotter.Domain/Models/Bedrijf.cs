namespace StageSpotter.Domain.Models;

public class Bedrijf
{
    public int Id { get; set; }
    public string Naam { get; set; }
    public string BedrijfUrl { get; set; }
        public string KvKNummer { get; set; }
        public string ContactPerson { get; set; }
        public string ContactEmail { get; set; }
    public ICollection<Vacature> Vacatures { get; set; }
}