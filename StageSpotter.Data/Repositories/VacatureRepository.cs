using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using StageSpotter.Data.Interfaces;
using StageSpotter.Data.DTOs;

namespace StageSpotter.Data.Repositories;

public class VacatureRepository : IVacatureRepository
{
    private readonly string _connectionString;

    public VacatureRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public VacatureDto? GetById(int id)
    {
        using (SqliteConnection connection = new SqliteConnection(_connectionString))
        {
            string sqlQuery = @"SELECT v.Id, v.Titel, v.Beschrijving, v.Locatie, v.PublicatieDatum, v.IsActief, v.SoortStage, v.VacatureUrl, v.BedrijfId, b.Naam as BedrijfNaam, b.BedrijfUrl FROM Vacatures v INNER JOIN Bedrijven b ON v.BedrijfId = b.Id WHERE v.Id = @Id";
            SqliteCommand command = new SqliteCommand(sqlQuery, connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var vacature = new VacatureDto
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Titel = reader["Titel"].ToString(),
                        Beschrijving = reader["Beschrijving"].ToString(),
                        Locatie = reader["Locatie"].ToString(),
                        PublicatieDatum = Convert.ToDateTime(reader["PublicatieDatum"]),
                        IsActief = Convert.ToBoolean(reader["IsActief"]),
                        SoortStageId = Convert.ToInt32(reader["SoortStage"]),
                        VacatureUrl = reader["VacatureUrl"].ToString(),
                        Bedrijf = new BedrijfDto
                        {
                            Id = Convert.ToInt32(reader["BedrijfId"]),
                            Naam = reader["BedrijfNaam"].ToString(),
                            BedrijfUrl = reader["BedrijfUrl"].ToString()
                        }
                    };
                    vacature.Opleidingsniveaus = GetOpleidingsniveausVoorVacature(vacature.Id);
                    vacature.Studierichtingen = GetStudierichtingenVoorVacature(vacature.Id);
                    return vacature;
                }
            }
        }
        return null;
    }

    public int Create(VacatureToRepositoryDto vacatureDto)
    {
        try
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                string sqlQuery = "INSERT INTO Vacatures (Titel, Beschrijving, Locatie, BedrijfId, PublicatieDatum, IsActief, SoortStage) VALUES (@Titel, @Beschrijving, @Locatie, @BedrijfId, @PublicatieDatum, @IsActief, @SoortStage); SELECT last_insert_rowid()";
                SqliteCommand command = new SqliteCommand(sqlQuery, connection);

                command.Parameters.AddWithValue("@Titel", vacatureDto.Titel);
                command.Parameters.AddWithValue("@Beschrijving", vacatureDto.Beschrijving);
                command.Parameters.AddWithValue("@Locatie", vacatureDto.Locatie);
                command.Parameters.AddWithValue("@BedrijfId", vacatureDto.BedrijfId);
                command.Parameters.AddWithValue("@PublicatieDatum", DateTime.Now);
                command.Parameters.AddWithValue("@IsActief", 1);
                command.Parameters.AddWithValue("@SoortStage", vacatureDto.SoortStageId);

                connection.Open();
                
                var newId = command.ExecuteScalar();

                if (newId == null)
                {
                    return -1;
                }

                return Convert.ToInt32(newId);
            }
        }
        catch (Exception ex)
        {
            throw new System.Data.DataException("Kon vacature niet aanmaken.", ex);
        }
    }

    public List<VacatureDto> GetVacaturesByBedrijf(string bedrijfsNaam)
    {
        try
        {
            List<VacatureDto> vacatures = new List<VacatureDto>();

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                string sqlQuery = @"
                    SELECT 
                        v.Id, v.Titel, v.Beschrijving, v.Locatie, v.PublicatieDatum, v.IsActief, v.SoortStage, v.VacatureUrl,
                        b.Id as BedrijfId, b.Naam as BedrijfNaam, b.BedrijfUrl
                    FROM Vacatures v
                    INNER JOIN Bedrijven b ON v.BedrijfId = b.Id
                    WHERE v.IsActief = 1 AND b.Naam = @BedrijfsNaam
                    ORDER BY v.PublicatieDatum DESC";
                
                SqliteCommand command = new SqliteCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@BedrijfsNaam", bedrijfsNaam);
                connection.Open();

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var vacature = new VacatureDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Titel = reader["Titel"].ToString(),
                            Beschrijving = reader["Beschrijving"].ToString(),
                            Locatie = reader["Locatie"].ToString(),
                            PublicatieDatum = Convert.ToDateTime(reader["PublicatieDatum"]),
                            IsActief = Convert.ToBoolean(reader["IsActief"]),
                            SoortStageId = Convert.ToInt32(reader["SoortStage"]),
                            VacatureUrl = reader["VacatureUrl"].ToString(),
                            Bedrijf = new BedrijfDto
                            { 
                                Id = Convert.ToInt32(reader["BedrijfId"]),
                                Naam = reader["BedrijfNaam"].ToString(),
                                BedrijfUrl = reader["BedrijfUrl"].ToString()
                            }
                        };
                        
                        vacature.Opleidingsniveaus = GetOpleidingsniveausVoorVacature(vacature.Id);
                        vacature.Studierichtingen = GetStudierichtingenVoorVacature(vacature.Id);
                        
                        vacatures.Add(vacature);
                    }
                }
            }
            return vacatures;
        }
        catch (Exception ex)
        {
            throw new System.Data.DataException("Kon vacatures voor bedrijf niet ophalen.", ex);
        }
    }

    public List<VacatureDto> GetVacatures()
    {
        try
        {
            List<VacatureDto> vacatures = new List<VacatureDto>();

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                string sqlQuery = @"
                    SELECT 
                        v.Id, v.Titel, v.Beschrijving, v.Locatie, v.PublicatieDatum, v.IsActief, v.SoortStage, v.VacatureUrl,
                        b.Id as BedrijfId, b.Naam as BedrijfNaam, b.BedrijfUrl
                    FROM Vacatures v
                    INNER JOIN Bedrijven b ON v.BedrijfId = b.Id
                    WHERE v.IsActief = 1
                    ORDER BY v.PublicatieDatum DESC";
                
                SqliteCommand command = new SqliteCommand(sqlQuery, connection);
                connection.Open();

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var vacature = new VacatureDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Titel = reader["Titel"].ToString(),
                            Beschrijving = reader["Beschrijving"].ToString(),
                            Locatie = reader["Locatie"].ToString(),
                            PublicatieDatum = Convert.ToDateTime(reader["PublicatieDatum"]),
                            IsActief = Convert.ToBoolean(reader["IsActief"]),
                            SoortStageId = Convert.ToInt32(reader["SoortStage"]),
                            VacatureUrl = reader["VacatureUrl"].ToString(),
                            Bedrijf = new BedrijfDto
                            { 
                                Id = Convert.ToInt32(reader["BedrijfId"]),
                                Naam = reader["BedrijfNaam"].ToString(),
                                BedrijfUrl = reader["BedrijfUrl"].ToString()
                            }
                        };
                        
                        // Laad opleidingsniveaus en studierichtingen
                        vacature.Opleidingsniveaus = GetOpleidingsniveausVoorVacature(vacature.Id);
                        vacature.Studierichtingen = GetStudierichtingenVoorVacature(vacature.Id);
                        
                        vacatures.Add(vacature);
                    }
                }
            }
            return vacatures;
        }
        catch (Exception ex)
        {
            throw new System.Data.DataException("Kon vacatures niet ophalen.", ex);
        }
    }

    private List<OpleidingsniveauDto> GetOpleidingsniveausVoorVacature(int vacatureId)
    {
        try
        {
            List<OpleidingsniveauDto> niveaus = new List<OpleidingsniveauDto>();
            
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                string sqlQuery = @"
                    SELECT o.Id, o.Niveau 
                    FROM Opleidingsniveaus o
                    INNER JOIN VacatureOpleidingsniveaus vo ON o.Id = vo.OpleidingsniveauId
                    WHERE vo.VacatureId = @VacatureId";
                
                SqliteCommand command = new SqliteCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@VacatureId", vacatureId);
                connection.Open();

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        niveaus.Add(new OpleidingsniveauDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Niveau = reader["Niveau"].ToString()
                        });
                    }
                }
            }
            return niveaus;
        }
        catch (Exception ex)
        {
            throw new System.Data.DataException("Kon opleidingsniveaus voor vacature niet ophalen.", ex);
        }
    }

    private List<StudierichtingDto> GetStudierichtingenVoorVacature(int vacatureId)
    {
        try
        {
            List<StudierichtingDto> richtingen = new List<StudierichtingDto>();
            
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                string sqlQuery = @"
                    SELECT s.Id, s.Richting 
                    FROM Studierichtingen s
                    INNER JOIN VacatureStudierichtingen vs ON s.Id = vs.StudierichtingId
                    WHERE vs.VacatureId = @VacatureId";
                
                SqliteCommand command = new SqliteCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@VacatureId", vacatureId);
                connection.Open();

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        richtingen.Add(new StudierichtingDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Richting = reader["Richting"].ToString()
                        });
                    }
                }
            }
            return richtingen;
        }
        catch (Exception ex)
        {
            throw new System.Data.DataException("Kon studierichtingen voor vacature niet ophalen.", ex);
        }
    }

    public bool Deactivate(int id, int bedrijfId)
    {
        try
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                string sqlQuery = "UPDATE Vacatures SET IsActief = 0 WHERE Id = @Id AND BedrijfId = @BedrijfId";
                SqliteCommand command = new SqliteCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@BedrijfId", bedrijfId);
                connection.Open();
                var rows = command.ExecuteNonQuery();
                return rows > 0;
            }
        }
        catch (Exception ex)
        {
            throw new DataException("Kon vacature niet deactiveren.", ex);
        }
    }

    public bool Update(VacatureToRepositoryDto dto, int bedrijfId)
    {
        try
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                string sqlQuery = @"UPDATE Vacatures SET Titel=@Titel, Beschrijving=@Beschrijving, Locatie=@Locatie, SoortStage=@SoortStage, VacatureUrl=@VacatureUrl WHERE Id=@Id AND BedrijfId=@BedrijfId";
                SqliteCommand command = new SqliteCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@Titel", dto.Titel);
                command.Parameters.AddWithValue("@Beschrijving", dto.Beschrijving);
                command.Parameters.AddWithValue("@Locatie", dto.Locatie);
                command.Parameters.AddWithValue("@SoortStage", dto.SoortStageId);
                command.Parameters.AddWithValue("@VacatureUrl", (object?)dto.VacatureUrl ?? DBNull.Value);
                command.Parameters.AddWithValue("@Id", dto.Id);
                command.Parameters.AddWithValue("@BedrijfId", bedrijfId);
                connection.Open();
                var rows = command.ExecuteNonQuery();
                return rows > 0;
            }
        }
        catch (Exception ex)
        {
            throw new DataException("Kon vacature niet bijwerken.", ex);
        }
    }
}
