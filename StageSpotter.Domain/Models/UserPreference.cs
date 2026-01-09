using System;

namespace StageSpotter.Domain.Models
{
    public class UserPreference
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Werkstijl { get; set; }
        public string Bedrijfstype { get; set; }
        public string Focus { get; set; }
        public string Leerdoel { get; set; }
    }
}
