using System;

namespace StageSpotter.Presentation.Models
{
    public class SavedVacatureViewModel
    {
        public int VacatureId { get; set; }
        public string Titel { get; set; } = string.Empty;
        public string Locatie { get; set; } = string.Empty;
        public string BedrijfNaam { get; set; } = string.Empty;
        public string SoortStage { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
