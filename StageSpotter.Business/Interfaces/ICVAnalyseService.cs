using StageSpotter.Domain.Models;
using System.Threading.Tasks;

namespace StageSpotter.Business.Interfaces
{
    public interface ICVAnalyseService 
    {
        Task AnalyseerCvAsync(CvDomain domainModel);
    }
}