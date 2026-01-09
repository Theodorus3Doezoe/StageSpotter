using System;

namespace StageSpotter.Domain.Models
{
    public class SavedVacature
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int VacatureId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
