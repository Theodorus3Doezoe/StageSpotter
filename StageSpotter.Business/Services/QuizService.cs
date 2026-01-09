using StageSpotter.Business.Interfaces;
using StageSpotter.Data.Interfaces;
using StageSpotter.Domain.Models;

namespace StageSpotter.Business.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;

        public QuizService(IQuizRepository quizRepository)
        {
            _quizRepository = quizRepository;
        }

        public void SavePreferences(UserPreference prefs)
        {
            _quizRepository.SavePreferences(prefs);
        }

        public UserPreference GetPreferencesByUserId(int userId)
        {
            return _quizRepository.GetPreferencesByUserId(userId);
        }
    }
}
