using System.Collections.Generic;
using System.Linq;
using StageSpotter.Business.Interfaces;
using StageSpotter.Data.Interfaces;
using StageSpotter.Domain.Models;

namespace StageSpotter.Business.Services
{
    public class MatchService : IMatchService
    {
        private readonly IVacatureRepository _vacatureRepo;
        private readonly ISavedAnalysisRepository _savedAnalysisRepo;
        private readonly IQuizRepository _quizRepository;

        public MatchService(IVacatureRepository vacatureRepo, ISavedAnalysisRepository savedAnalysisRepo, IQuizRepository quizRepository)
        {
            _vacatureRepo = vacatureRepo;
            _savedAnalysisRepo = savedAnalysisRepo;
            _quizRepository = quizRepository;
        }

        public List<VacatureScoreItem> GetMatchesForUser(int userId)
        {
            var vacatures = _vacatureRepo.GetVacatures();
            var prefs = _quizRepository.GetPreferencesByUserId(userId);
            var analyses = _savedAnalysisRepo.GetByUserId(userId) ?? new System.Collections.Generic.List<StageSpotter.Domain.Models.SavedAnalysis>();

            var combinedText = string.Join(" ", analyses.Select(a => a.Result ?? string.Empty));
            combinedText += " ";
            if (prefs != null)
            {
                combinedText += (prefs.Focus ?? string.Empty) + " " + (prefs.Werkstijl ?? string.Empty) + " " + (prefs.Bedrijfstype ?? string.Empty) + " " + (prefs.Leerdoel ?? string.Empty);
            }

            var tokens = Tokenize(combinedText);

            var list = new List<VacatureScoreItem>();
            foreach (var v in vacatures)
            {
                var bedrijfNaam = v.Bedrijf != null ? v.Bedrijf.Naam : string.Empty;
                var text = (v.Titel ?? string.Empty) + " " + (v.Beschrijving ?? string.Empty) + " " + (v.Locatie ?? string.Empty) + " " + bedrijfNaam;
                
                // Bereken score: tel hoe vaak elk token voorkomt in de vacature text
                int score = 0;
                foreach (string token in tokens)
                {
                    int aantalKeer = CountOccurrences(text, token);
                    score += aantalKeer;
                }

                // small bonus when vacature contains preferred focus or bedrijfstype
                if (prefs != null)
                {
                    if (!string.IsNullOrWhiteSpace(prefs.Focus) && text.ToLower().Contains(prefs.Focus.ToLower())) score += 3;
                    if (!string.IsNullOrWhiteSpace(prefs.Bedrijfstype) && text.ToLower().Contains(prefs.Bedrijfstype.ToLower())) score += 2;
                }

                list.Add(new VacatureScoreItem { VacatureId = v.Id, Titel = v.Titel, BedrijfNaam = bedrijfNaam, Score = score });
            }

            // Sorteer vacatures op score (hoogste eerst) en neem top 10
            list.Sort((a, b) => b.Score.CompareTo(a.Score));
            
            List<VacatureScoreItem> top10 = new List<VacatureScoreItem>();
            for (int i = 0; i < list.Count && i < 10; i++)
            {
                top10.Add(list[i]);
            }
            
            return top10;
        }

        private List<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new List<string>();
            }
            
            // Maak text lowercase en haal alleen letters, cijfers en spaties over
            string cleaned = "";
            foreach (char c in text)
            {
                if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                {
                    cleaned += char.ToLower(c);
                }
            }
            
            // Split op spaties en verwijder lege strings
            string[] parts = cleaned.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            
            // Tel hoe vaak elk woord voorkomt
            Dictionary<string, int> woordTelling = new Dictionary<string, int>();
            foreach (string woord in parts)
            {
                if (woordTelling.ContainsKey(woord))
                {
                    woordTelling[woord]++;
                }
                else
                {
                    woordTelling[woord] = 1;
                }
            }
            
            // Sorteer woorden op aantal keren voorkomen (meest voorkomende eerst)
            List<KeyValuePair<string, int>> gesorteerd = new List<KeyValuePair<string, int>>(woordTelling);
            gesorteerd.Sort((a, b) => b.Value.CompareTo(a.Value));
            
            // Neem alleen de top 50 meest voorkomende woorden
            List<string> topWoorden = new List<string>();
            for (int i = 0; i < gesorteerd.Count && i < 50; i++)
            {
                topWoorden.Add(gesorteerd[i].Key);
            }
            
            return topWoorden;
        }

        private int CountOccurrences(string text, string token)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(token)) return 0;
            var t = text.ToLower();
            var idx = 0;
            var count = 0;
            while ((idx = t.IndexOf(token, idx)) != -1)
            {
                count++;
                idx += token.Length;
            }
            return count;
        }
    }
}
