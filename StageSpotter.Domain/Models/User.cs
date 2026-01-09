namespace StageSpotter.Domain.Models
{
    public enum UserType
    {
        Student = 0,
        Bedrijf = 1
    }

    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserType Type { get; set; } = UserType.Student;
        public int? BedrijfId { get; set; } // FK naar Bedrijven tabel
    }
}
