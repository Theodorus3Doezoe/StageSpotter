using Microsoft.Extensions.Configuration;
using StageSpotter.Business.Interfaces;
using Google.GenAI;        
using Google.GenAI.Types;  

namespace StageSpotter.Business.Services
{
    public class GeminiService : IAIService
    {
        private readonly string _apiKey;

        public GeminiService(IConfiguration configuration)
        {
            _apiKey = configuration["Gemini:ApiKey"];
        }

        public async Task<string> AnalyzeAsync(string cvText)
        {
            if (string.IsNullOrEmpty(_apiKey)) return "API Key ontbreekt in configuratie.";

            // Lees de prompt template
            string promptTemplate;
            try
            {
                var promptPath = Path.Combine(AppContext.BaseDirectory, "Prompts", "cv_prompt.txt");
                promptTemplate = await System.IO.File.ReadAllTextAsync(promptPath);
            }
            catch (Exception ex)
            {
                // Fallback als bestand niet gevonden wordt
                Console.WriteLine($"Prompt file error: {ex.Message}");
                promptTemplate = "Analyseer dit CV en geef feedback:\n\n{0}";
            }

            // Combineer template met CV tekst
            // We gebruiken string.Format of replace, afhankelijk van hoe de txt eruit ziet.
            // De txt eindigt met {0}, dus string.Format is prima.
            var fullPrompt = promptTemplate.Replace("{0}", cvText);

            // Maak de client
            var client = new Client(apiKey: _apiKey);

            try
            {
                // We proberen het '8b' model, dit is vaak de gratis/goedkope variant
                // Als dit ook niet werkt, probeer dan: "gemini-1.5-flash-001"
                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-2.5-flash-lite", 
                    contents: fullPrompt
                );

                if (response?.Candidates != null && response.Candidates.Count > 0)
                {
                    // Het antwoord veilig ophalen
                    foreach (var part in response.Candidates[0].Content.Parts)
                    {
                        if (!string.IsNullOrEmpty(part.Text))
                        {
                            return part.Text;
                        }
                    }
                }
                
                return "Geen tekst ontvangen van AI.";
            }
            catch (Exception ex)
            {
                // Vangt ook de '404 Not Found' of 'Quota Exceeded' af
                return $"Fout bij AI aanroep: {ex.Message}";
            }
        }
    }
}