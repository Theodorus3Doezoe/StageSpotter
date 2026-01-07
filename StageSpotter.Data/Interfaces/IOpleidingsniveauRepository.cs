using StageSpotter.Data.DTOs;

namespace StageSpotter.Data.Interfaces
{
    public interface IOpleidingsniveauRepository
    {
        List<OpleidingsniveauDto> GetAlleOpleidingsniveaus();
        void AddVacatureNiveau(int vacatureId, int niveauId);
    }
}