namespace StageSpotter.Web.Models
{
        public class VacatureLijstItem
    {
        public int Id { get; set; }
        public string Titel { get; set; }
        public string BedrijfNaam { get; set; }
        public string Locatie { get; set; }
        public string Opleidingen { get; set; } 
        public string Studierichtingen { get; set; } 
        public DateTime PublicatieDatum { get; set; }
    }
}