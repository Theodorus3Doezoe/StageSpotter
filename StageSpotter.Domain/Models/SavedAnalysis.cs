using System;

namespace StageSpotter.Domain.Models
{
    public class SavedAnalysis
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FileName { get; set; }
        public string Result { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
