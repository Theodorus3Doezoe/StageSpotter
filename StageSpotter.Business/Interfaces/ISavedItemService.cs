using System.Collections.Generic;
using StageSpotter.Domain.Models;

namespace StageSpotter.Business.Interfaces
{
    public interface ISavedItemService
    {
        int SaveAnalysis(int userId, string fileName, string result);
        IEnumerable<SavedAnalysis> GetAnalysesForUser(int userId);
        SavedAnalysis? GetAnalysisById(int id);

        int SaveVacature(int userId, int vacatureId);
        IEnumerable<SavedVacature> GetVacaturesForUser(int userId);
        int RemoveVacature(int userId, int vacatureId);
    }
}
