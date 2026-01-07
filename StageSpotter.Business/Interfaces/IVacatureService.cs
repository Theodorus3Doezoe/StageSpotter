using StageSpotter.Domain.Models;

namespace StageSpotter.Business.Interfaces
{
    public interface IVacatureService
    {
        void CreateVacature(Vacature vacature);
        List<Opleidingsniveau> GetAlleOpleidingsniveaus();
        List<Studierichting> GetAlleStudierichtingen();
        List<Vacature> GetAlleVacatures();
    }
}