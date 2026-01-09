using StageSpotter.Data.DTOs;

namespace StageSpotter.Data.Interfaces
{
    public interface IBedrijfRepository
    {
        BedrijfDto? FindByName(string bedrijfsnaam);
        BedrijfDto Create(BedrijfDto bedrijfDto);
        System.Collections.Generic.List<BedrijfDto> GetAll();
        BedrijfDto? GetById(int id);
    }
}