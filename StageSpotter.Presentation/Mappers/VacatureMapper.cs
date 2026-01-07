using StageSpotter.Web.Models;
using StageSpotter.Domain.Models;
using StageSpotter.Domain.Enums;

namespace StageSpotter.Web.Mappers;

public static class VacatureMapper
{
    public static Vacature ToModel(CreateVacatureViewModel viewModel)
    {
        var vacature = new Vacature
        {
            Titel = viewModel.Titel,
            Beschrijving = viewModel.Beschrijving,
            Locatie = viewModel.Locatie,            
            PublicatieDatum = DateTime.Now,
            IsActief = true,
            SoortStage = (SoortStage)viewModel.SoortStageId,
            Bedrijf = new Bedrijf
            {
                Naam = viewModel.Bedrijfnaam
            }
        };

        if (viewModel.OpleidingsniveauIds != null)
        {
            foreach (var id in viewModel.OpleidingsniveauIds)
            {
                vacature.Opleidingsniveaus.Add(new Opleidingsniveau { Id = id });
            }
        }

        if (viewModel.StudierichtingIds != null)
        {
            foreach (var id in viewModel.StudierichtingIds)
            {
                vacature.Studierichtingen.Add(new Studierichting { Id = id });
            }
        }

        return vacature;
    }
}