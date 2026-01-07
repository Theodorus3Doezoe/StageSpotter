namespace StageSpotter.Business.Interfaces
{
    public interface IAIService {
        Task<string> AnalyzeAsync(string text);
    }
    
}