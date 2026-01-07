using StageSpotter.Business.Services;
using StageSpotter.Data.Interfaces;
using StageSpotter.Business.Interfaces;

namespace StageSpotter.Business.Builders
{
    public class VacatureServiceBuilder
    {
        public required IVacatureRepository VacatureRepo { get; init; }
        public required IOpleidingsniveauRepository OpleidingRepo { get; init; }
        public required IStudierichtingRepository StudieRepo { get; init; }
        public required IBedrijfRepository BedrijfRepo { get; init; }

        public VacatureService Build()
        {
            return new VacatureService(
                VacatureRepo, 
                OpleidingRepo, 
                StudieRepo, 
                BedrijfRepo
            );
        }
    }
}
