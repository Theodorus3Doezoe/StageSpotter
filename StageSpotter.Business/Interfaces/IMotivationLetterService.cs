using System.Threading.Tasks;

namespace StageSpotter.Business.Interfaces
{
    public interface IMotivationLetterService
    {
        Task<string> GenerateMotivationLetterAsync(int userId, int vacatureId);
    }
}
