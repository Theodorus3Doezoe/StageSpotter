using StageSpotter.Data.DTOs;
using StageSpotter.Domain.Models;
using StageSpotter.Domain.Enums;

namespace StageSpotter.Business.Mappers;

public static class VacatureMapper
{
    public static VacatureToRepositoryDto ToRepositoryDto(Vacature vacature)
    {
        return new VacatureToRepositoryDto
        {
            Titel = vacature.Titel,
            Beschrijving = vacature.Beschrijving,
            Locatie = vacature.Locatie,
            BedrijfId = vacature.BedrijfId,
            PublicatieDatum = vacature.PublicatieDatum,
            IsActief = vacature.IsActief,
            SoortStageId = (int)vacature.SoortStage,
            VacatureUrl = vacature.VacatureUrl ?? ""
        };
    }

    public static Vacature ToDomain(VacatureDto dto)
    {
        return new Vacature
        {
            Id = dto.Id,
            Titel = dto.Titel,
            Beschrijving = dto.Beschrijving,
            Locatie = dto.Locatie,
            PublicatieDatum = dto.PublicatieDatum,
            IsActief = dto.IsActief,
            SoortStage = (SoortStage)dto.SoortStageId,
            VacatureUrl = dto.VacatureUrl,
            Bedrijf = new Bedrijf { Id = dto.Bedrijf.Id, Naam = dto.Bedrijf.Naam }
        };
    }
    
    public static Opleidingsniveau ToDomain(OpleidingsniveauDto dto)
    {
        return new Opleidingsniveau { Id = dto.Id, Niveau = dto.Niveau };
    }

    public static Studierichting ToDomain(StudierichtingDto dto)
    {
        return new Studierichting { Id = dto.Id, Richting = dto.Richting };
    }
}