namespace StageSpotter.Domain.Models;

public class Studierichting
{
    public int Id { get; set; }
    public string Richting { get; set; }
    public ICollection<Vacature> Vacatures { get; set; }
}