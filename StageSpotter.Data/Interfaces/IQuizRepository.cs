using StageSpotter.Domain.Models;

namespace StageSpotter.Data.Interfaces
{
    public interface IQuizRepository
    {
        void SavePreferences(UserPreference prefs);
        UserPreference GetPreferencesByUserId(int userId);
    }
}
