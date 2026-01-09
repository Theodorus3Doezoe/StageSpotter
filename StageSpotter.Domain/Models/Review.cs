using System;

namespace StageSpotter.Domain.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BedrijfId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
