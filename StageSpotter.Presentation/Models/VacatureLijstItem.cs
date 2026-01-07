namespace StageSpotter.Web.Models
{
    public class VacatureDetailItem
    {
        public int Id { get; set; }
        public string Titel { get; set; }
        public string BedrijfNaam { get; set; }
        public string Locatie { get; set; }
        public string SoortStage { get; set; }
        public string Beschrijving { get; set; }
        public string? VacatureUrl { get; set; }
        public string Opleidingen { get; set; } 
        public string Studierichtingen { get; set; } 
    }
}