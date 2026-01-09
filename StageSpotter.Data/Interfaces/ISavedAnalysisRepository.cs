using System.Collections.Generic;
using StageSpotter.Domain.Models;

namespace StageSpotter.Data.Interfaces
{
    public interface ISavedAnalysisRepository
    {
        int Create(SavedAnalysis analysis);
        IEnumerable<SavedAnalysis> GetByUserId(int userId);
        SavedAnalysis? GetById(int id);
    }
}
