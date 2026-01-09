using System;
using System.Threading.Tasks;
using StageSpotter.Business.Interfaces;
using StageSpotter.Data.Interfaces;

namespace StageSpotter.Business.Services
{
    public class MotivationLetterService : IMotivationLetterService
    {
        private readonly ISavedAnalysisRepository _savedAnalysisRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly IVacatureRepository _vacatureRepository;
        private readonly IAIService _aiService;

        public MotivationLetterService(
            ISavedAnalysisRepository savedAnalysisRepository,
            IQuizRepository quizRepository,
            IVacatureRepository vacatureRepository,
            IAIService aiService)
        {
            _savedAnalysisRepository = savedAnalysisRepository;
            _quizRepository = quizRepository;
            _vacatureRepository = vacatureRepository;
            _aiService = aiService;
        }

        public async Task<string> GenerateMotivationLetterAsync(int userId, int vacatureId)
        {
            // Get CV analysis
            var analyses = _savedAnalysisRepository.GetByUserId(userId);
            string cvContent;
            
            if (analyses != null && analyses.Count() > 0)
            {
                // Verzamel alle resultaten in een lijst
                var resultaten = new System.Collections.Generic.List<string>();
                foreach (var analyse in analyses)
                {
                    if (!string.IsNullOrEmpty(analyse.Result))
                    {
                        resultaten.Add(analyse.Result);
                    }
                    else
                    {
                        resultaten.Add(string.Empty);
                    }
                }
                cvContent = string.Join("\n\n", resultaten);
            }
            else
            {
                cvContent = "Geen CV analyse beschikbaar";
            }

            // Get user preferences
            var prefs = _quizRepository.GetPreferencesByUserId(userId);
            string prefsText;
            
            if (prefs != null)
            {
                prefsText = $"Werkstijl: {prefs.Werkstijl}\nBedrijfstype: {prefs.Bedrijfstype}\nFocus: {prefs.Focus}\nLeerdoel: {prefs.Leerdoel}";
            }
            else
            {
                prefsText = "Geen voorkeuren opgegeven";
            }

            // Get vacancy details
            var vacatures = _vacatureRepository.GetVacatures();
            
            // Zoek de vacature met het gegeven id
            StageSpotter.Data.DTOs.VacatureDto vacature = null;
            if (vacatures != null)
            {
                foreach (var v in vacatures)
                {
                    if (v.Id == vacatureId)
                    {
                        vacature = v;
                        break;
                    }
                }
            }
            
            if (vacature == null)
                throw new ArgumentException("Vacature niet gevonden");

            var vacatureText = $"Titel: {vacature.Titel}\nBedrijf: {vacature.Bedrijf?.Naam}\nBeschrijving: {vacature.Beschrijving}\nLocatie: {vacature.Locatie}";

            // Create prompt for motivation letter
            var prompt = $@"Schrijf een professionele motivatiebrief/solicitatiebrief voor een stageplek. 

Gebruiker CV samenvatting:
{cvContent}

Gebruiker preferenties:
{prefsText}

Vacature informatie:
{vacatureText}

Schrijf een persoonlijke, enthousiaste motivatiebrief (300-400 woorden) waarin je aantoont:
1. Waarom je geïnteresseerd bent in deze specifieke stage
2. Hoe je skills en voorkeuren aansluiten op de vacature
3. Wat je wilt leren en bijdragen

Formatteer het als een echte solicitatiebrief met aanhef, inhoud en afscheiding. Begin met ""Geachte,""";

            var letter = await _aiService.AnalyzeAsync(prompt);
            return letter;
        }
    }
}
