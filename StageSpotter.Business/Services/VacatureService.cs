using StageSpotter.Business.Interfaces;
using StageSpotter.Business.Mappers;
using StageSpotter.Data.Interfaces;
using StageSpotter.Domain.Models;
using StageSpotter.Data.DTOs;

namespace StageSpotter.Business.Services
{    public class VacatureService(
        IVacatureRepository vacatureRepository,
        IOpleidingsniveauRepository opleidingsniveauRepository,
        IStudierichtingRepository studierichtingRepository,
        IBedrijfRepository bedrijfRepository
        ) : IVacatureService
    {
        public void CreateVacature(Vacature vacature)
        {            
            var existingVacatures = vacatureRepository.GetVacaturesByBedrijf(vacature.Bedrijf.Naam);
            if (existingVacatures.Any(v => v.Titel.Equals(vacature.Titel, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Deze vacature bestaat al bij dit bedrijf.");
            }

            var bestaandBedrijf = bedrijfRepository.FindByName(vacature.Bedrijf.Naam);
            
            int bedrijfId;
            if (bestaandBedrijf != null)
            {
                bedrijfId = bestaandBedrijf.Id;
            }
            else
            {
                var bedrijfDto = new BedrijfDto
                {
                    Naam = vacature.Bedrijf.Naam,
                    BedrijfUrl = ""
                };
                bedrijfId = bedrijfRepository.Create(bedrijfDto).Id;
            }

            var nieuweVacature = new Vacature
            {
                Titel = vacature.Titel,
                Beschrijving = vacature.Beschrijving,
                Locatie = vacature.Locatie,
                BedrijfId = bedrijfId,
                PublicatieDatum = DateTime.Now,
                IsActief = true,
                SoortStage = vacature.SoortStage,
                VacatureUrl = "",
            };

            var repositoryDto = VacatureMapper.ToRepositoryDto(nieuweVacature);
            int newVacatureId = vacatureRepository.Create(repositoryDto);

            foreach (var niveau in vacature.Opleidingsniveaus)
            {
                opleidingsniveauRepository.AddVacatureNiveau(newVacatureId, niveau.Id);
            }

            foreach (var richting in vacature.Studierichtingen)
            {
                studierichtingRepository.AddVacatureStudierichting(newVacatureId, richting.Id);
            }
        }

        public List<Vacature> GetAlleVacatures()
        {
            var dtos = vacatureRepository.GetVacatures();
            return dtos.Select(VacatureMapper.ToDomain).ToList();
        }

        public Vacature? GetVacatureById(int id)
        {
            var dto = vacatureRepository.GetById(id);
            return dto == null ? null : VacatureMapper.ToDomain(dto);
        }

        public List<Opleidingsniveau> GetAlleOpleidingsniveaus()
        {
            var dtos = opleidingsniveauRepository.GetAlleOpleidingsniveaus();
            return dtos.Select(VacatureMapper.ToDomain).ToList();
        }

        public List<Studierichting> GetAlleStudierichtingen()
        {
            var dtos = studierichtingRepository.GetAlleStudierichtingen();
            return dtos.Select(VacatureMapper.ToDomain).ToList();
        }

        public bool UpdateVacature(Vacature vacature, int bedrijfId)
        {
            var dto = VacatureMapper.ToRepositoryDto(vacature);
            dto.Id = vacature.Id;
            return vacatureRepository.Update(dto, bedrijfId);
        }

        public bool DeactivateVacature(int id, int bedrijfId)
        {
            return vacatureRepository.Deactivate(id, bedrijfId);
        }
    }
}