using System.Collections.Generic;
using StageSpotter.Domain.Models;

namespace StageSpotter.Business.Interfaces
{
    public interface IMatchService
    {
        List<VacatureScoreItem> GetMatchesForUser(int userId);
    }
}
