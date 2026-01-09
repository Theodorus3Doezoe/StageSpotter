using StageSpotter.Domain.Models;

namespace StageSpotter.Data.Interfaces
{
    public interface ISavedMotivationLetterRepository
    {
        void SaveMotivationLetter(int userId, int vacatureId, string content, string vacatureTitel, string bedrijfNaam);
        List<dynamic> GetByUserId(int userId);
        void DeleteById(int id);
    }
}
