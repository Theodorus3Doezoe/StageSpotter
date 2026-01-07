using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;
using StageSpotter.Business.Interfaces;

namespace StageSpotter.Business.Services
{
    public class FileService : IFileService
    {
        public string ExtractText(Stream fileStream)
        {
            try
            {
                // OpenXml verwacht dat de stream leesbaar is
                using (var wordDoc = WordprocessingDocument.Open(fileStream, false))
                {
                    var body = wordDoc.MainDocumentPart?.Document.Body;
                    var sb = new StringBuilder();
                    foreach (var paragraph in body.Descendants<Paragraph>())
                    {
                        if (!string.IsNullOrWhiteSpace(paragraph.InnerText))
                        {
                            sb.AppendLine(paragraph.InnerText);
                        }
                    }
                    return sb.ToString();
                }
            }
            catch
            {
                return ""; 
            }
        }
    }
}
