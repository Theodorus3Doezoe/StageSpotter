using StageSpotter.Data.DTOs;

namespace StageSpotter.Data.Interfaces
{
    public interface IStudierichtingRepository
    {
        List<StudierichtingDto> GetAlleStudierichtingen();
        void AddVacatureStudierichting(int vacatureId, int studierichtingId);
    }
}