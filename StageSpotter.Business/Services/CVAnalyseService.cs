using StageSpotter.Business.Interfaces;
using StageSpotter.Domain.Models;
using System.Reflection; // <--- NODIG voor het vinden van het bestand

namespace StageSpotter.Business.Services
{
    public class CVAnalyseService : ICVAnalyseService
    {
        private readonly IFileService _fileService;
        private readonly IAIService _aiService;

        public CVAnalyseService(IFileService fileService, IAIService aiService)
        {
            _fileService = fileService;
            _aiService = aiService;
        }

        public async Task AnalyseerCvAsync(CvDomain domainModel)
        {
            // Tekst uit bestand halen
            if (domainModel.BestandsStream == null) 
            {
                domainModel.AiAnalyse = "Geen bestand gevonden.";
                return;
            }

            var cvTekst = _fileService.ExtractText(domainModel.BestandsStream);
            domainModel.GeextraheerdeTekst = cvTekst; 

            if (string.IsNullOrWhiteSpace(cvTekst))
            {
                domainModel.AiAnalyse = "Kon geen leesbare tekst vinden in het bestand.";
                return;
            }

            // 2. Prompt inladen 
            string promptTemplate = LoadPromptFromFile("cv_prompt.txt");

            // CV tekst in de template injecteren
            // Dit vervangt de {0} in je tekstbestand met de echte CV tekst
            string volledigePrompt = string.Format(promptTemplate, cvTekst);

            // 4. Naar AI sturen (We sturen nu de volledige instructie + cv)
            var analyse = await _aiService.AnalyzeAsync(volledigePrompt);
            
            // 5. Resultaat opslaan
            domainModel.AiAnalyse = analyse;
        }

        // Hulpfunctie om het bestand veilig te vinden in de 'bin' map
        private string LoadPromptFromFile(string filename)
        {
            try 
            {
                // Dit pakt het pad waar de DLL van je applicatie staat
                var buildDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                
                // Plakt daar 'Prompts' en de bestandsnaam achter
                var filePath = Path.Combine(buildDir, "Prompts", filename);

                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
                
                return $"FOUT: Prompt bestand niet gevonden op: {filePath}. Controleer je .csproj copy settings!";
            }
            catch (Exception ex)
            {
                return $"FOUT bij laden prompt: {ex.Message}";
            }
        }
    }
}
