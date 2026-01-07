using System.IO;

namespace StageSpotter.Domain.Models
{
    public class CvDomain
{
    public string BestandsNaam { get; set; }
    public Stream BestandsStream { get; set; } 
    public string? GeextraheerdeTekst { get; set; } 
    public string? AiAnalyse { get; set; }          
}
}