namespace StageSpotter.Domain.Models;

public class Opleidingsniveau
{
    public int Id { get; set; }
    public string Niveau { get; set; }
    public ICollection<Vacature> Vacatures { get; set; }
}