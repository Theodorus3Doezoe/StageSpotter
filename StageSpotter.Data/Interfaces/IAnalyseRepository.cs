using System.Collections.Generic;

namespace StageSpotter.Data.Interfaces
{
    public interface IAnalyseRepository
    {
        void SaveAnalyse(int gebruikerId, string cvBestandsnaam, string resultaat);
        List<(string CvBestandsnaam, string Resultaat, string AnalyseDatum)> GetAnalyses(int gebruikerId);
    }
}
