using StageSpotter.Data.DTOs;

namespace StageSpotter.Data.Interfaces
{
    public interface IVacatureRepository
    {
        List<VacatureDto> GetVacatures(); 
        List<VacatureDto> GetVacaturesByBedrijf(string bedrijfsNaam);
        int Create(VacatureToRepositoryDto VacatureToRepoDto);
        VacatureDto? GetById(int id);
        bool Deactivate(int id, int bedrijfId);
        bool Update(VacatureToRepositoryDto dto, int bedrijfId);
    }
}