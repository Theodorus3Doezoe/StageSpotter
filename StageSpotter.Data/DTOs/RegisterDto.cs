namespace StageSpotter.Data.DTOs
{
    public class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    public bool IsBedrijf { get; set; }
    public string? Bedrijfsnaam { get; set; }
    public string? BedrijfUrl { get; set; }
    public string? KvKNummer { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactEmail { get; set; }
    }
}
