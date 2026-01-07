using Moq;
using StageSpotter.Business.Services;
using StageSpotter.Data.DTOs;
using StageSpotter.Data.Interfaces;
using StageSpotter.Domain.Models;
using Xunit;
using System.Collections.Generic;

namespace StageSpotter.Tests.Services
{
    public class VacatureServiceTests
    {
        private readonly Mock<IVacatureRepository> _mockVacatureRepo;
        private readonly Mock<IOpleidingsniveauRepository> _mockNiveauRepo;
        private readonly Mock<IStudierichtingRepository> _mockRichtingRepo;
        private readonly Mock<IBedrijfRepository> _mockBedrijfRepo;

        private readonly VacatureService _service;

        public VacatureServiceTests()
        {
            _mockVacatureRepo = new Mock<IVacatureRepository>();
            _mockNiveauRepo = new Mock<IOpleidingsniveauRepository>();
            _mockRichtingRepo = new Mock<IStudierichtingRepository>();
            _mockBedrijfRepo = new Mock<IBedrijfRepository>();

            _service = new VacatureService(
                _mockVacatureRepo.Object,
                _mockNiveauRepo.Object,
                _mockRichtingRepo.Object,
                _mockBedrijfRepo.Object
            );
        }

        [Fact]
        public void CreateVacature_BestaandBedrijf()
        {
            // Arange
            var vacatureModel = MaakTestVacature("Bestaand Bedrijf B.V.");

            _mockBedrijfRepo.Setup(repo => repo.FindByName("Bestaand Bedrijf B.V."))
                            .Returns(new BedrijfDto { Id = 10, Naam = "Bestaand Bedrijf B.V." });

            _mockVacatureRepo.Setup(repo => repo.Create(It.IsAny<VacatureToRepositoryDto>()))
                             .Returns(500);
            
            // Act
            _service.CreateVacature(vacatureModel);

            //Dto testen
            // Bedrijf aanmaken en vacature loskoppelen, onderzoeken

            // Assert
            _mockBedrijfRepo.Verify(repo => repo.Create(It.IsAny<BedrijfDto>()), Times.Never);

            _mockVacatureRepo.Verify(repo => repo.Create(It.IsAny<VacatureToRepositoryDto>()), Times.Once);

            _mockNiveauRepo.Verify(repo => repo.AddVacatureNiveau(500, It.IsAny<int>()), Times.Once);
            _mockRichtingRepo.Verify(repo => repo.AddVacatureStudierichting(500, It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void CreateVacature_NieuwBedrijf()
        {
            // Arange
            var vacatureModel = MaakTestVacature("Gloednieuw Bedrijf");

            
            _mockBedrijfRepo.Setup(repo => repo.FindByName("Gloednieuw Bedrijf"))
                            .Returns((BedrijfDto?)null);

            _mockBedrijfRepo.Setup(repo => repo.Create(It.IsAny<BedrijfDto>()))
                            .Returns(new BedrijfDto { Id = 99, Naam = "Gloednieuw Bedrijf" });

            _mockVacatureRepo.Setup(repo => repo.Create(It.IsAny<VacatureToRepositoryDto>()))
                             .Returns(500);

            // Act
            _service.CreateVacature(vacatureModel);

            // Assert
            _mockBedrijfRepo.Verify(repo => repo.Create(It.IsAny<BedrijfDto>()), Times.Once);

            _mockVacatureRepo.Verify(repo => repo.Create(It.IsAny<VacatureToRepositoryDto>()), Times.Once);
        }

        [Fact]
        public void GetAlleVacatures_GeeftLijst()
        {
            var dbLijst = new List<VacatureDto>
            {
                new VacatureDto 
                { 
                    Id = 1, 
                    Titel = "Test Vacature", 
                    Bedrijf = new BedrijfDto 
                    { 
                        Naam = "TestBedrijf",
                        BedrijfUrl = "http://test.nl"
                    } 
                }
            };

            _mockVacatureRepo.Setup(repo => repo.GetVacatures()).Returns(dbLijst);

            var resultaat = _service.GetAlleVacatures();

            Assert.NotNull(resultaat);
            Assert.Single(resultaat);
            Assert.Equal("Test Vacature", resultaat[0].Titel);
            
            _mockVacatureRepo.Verify(repo => repo.GetVacatures(), Times.Once);
        }

        [Fact]
        public void GetAlleOpleidingsniveaus_GeeftLijst()
        {
            var dbLijst = new List<OpleidingsniveauDto>
            {
                new OpleidingsniveauDto { Id = 1, Niveau = "MBO" },
                new OpleidingsniveauDto { Id = 2, Niveau = "HBO" }
            };

            _mockNiveauRepo.Setup(repo => repo.GetAlleOpleidingsniveaus()).Returns(dbLijst);

            var resultaat = _service.GetAlleOpleidingsniveaus();

            Assert.Equal(2, resultaat.Count);
            Assert.Equal("MBO", resultaat[0].Niveau);
            
            _mockNiveauRepo.Verify(repo => repo.GetAlleOpleidingsniveaus(), Times.Once);
        }

        [Fact]
        public void GetAlleStudierichtingen_GeeftLijst()
        {
            var dbLijst = new List<StudierichtingDto>
            {
                new StudierichtingDto { Id = 1, Richting = "ICT" },
                new StudierichtingDto { Id = 2, Richting = "Marketing" }
            };

            _mockRichtingRepo.Setup(repo => repo.GetAlleStudierichtingen()).Returns(dbLijst);

            var resultaat = _service.GetAlleStudierichtingen();

            Assert.Equal(2, resultaat.Count);
            Assert.Equal("ICT", resultaat[0].Richting);
            
            _mockRichtingRepo.Verify(repo => repo.GetAlleStudierichtingen(), Times.Once);
        }
        private Vacature MaakTestVacature(string bedrijfsnaam)
        {
            return new Vacature
            {
                Titel = "Stage Developer",
                Bedrijf = new Bedrijf { Naam = bedrijfsnaam },
                Beschrijving = "Test omschrijving",
                Locatie = "Eindhoven",
                Opleidingsniveaus = new List<Opleidingsniveau> { new Opleidingsniveau { Id = 1, Niveau = "HBO" } },
                Studierichtingen = new List<Studierichting> { new Studierichting { Id = 2, Richting = "ICT" } }
            };
        }
    }
}