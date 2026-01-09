using System.Collections.Generic;
using StageSpotter.Domain.Models;

namespace StageSpotter.Data.Interfaces
{
    public interface ISavedVacatureRepository
    {
        int Create(SavedVacature savedVacature);
        IEnumerable<SavedVacature> GetByUserId(int userId);
        int Delete(int userId, int vacatureId);
    }
}
