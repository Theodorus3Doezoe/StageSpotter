using StageSpotter.Domain.Models;

namespace StageSpotter.Business.Interfaces
{
    public interface IQuizService
    {
        void SavePreferences(UserPreference prefs);
        UserPreference GetPreferencesByUserId(int userId);
    }
}
