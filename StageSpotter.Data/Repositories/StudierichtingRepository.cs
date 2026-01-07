using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using StageSpotter.Data.Interfaces;
using StageSpotter.Data.DTOs;

namespace StageSpotter.Data.Repositories;

public class StudierichtingRepository : IStudierichtingRepository
{
    private readonly string _connectionString;

    public StudierichtingRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'");
    }
    
    // Maak de koppeling tussen vacature en studierichting 
    public void AddVacatureStudierichting(int vacatureId, int studierichtingId)
    {
        try
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                string SqlQuery = 
                    "INSERT INTO VacatureStudierichtingen (VacatureId, StudierichtingId) VALUES (@VacatureId, @StudierichtingId)";
                
                SqliteCommand command = new SqliteCommand(SqlQuery, connection);
                
                command.Parameters.AddWithValue("@VacatureId", vacatureId);
                command.Parameters.AddWithValue("@StudierichtingId", studierichtingId);
                
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new System.Data.DataException("Kon vacature studierichting niet toevoegen aan vacature.", ex);
        }
    }

    public List<StudierichtingDto> GetAlleStudierichtingen()
    {
        try
        {
            List<StudierichtingDto> niveaus = new List<StudierichtingDto>();

            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                string sqlQuery = "SELECT ID, Richting FROM Studierichtingen ORDER BY Richting";
             
                SqliteCommand command = new SqliteCommand(sqlQuery, connection);
            
                connection.Open();

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var richting = new StudierichtingDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Richting = reader["Richting"].ToString()
                        };
                        niveaus.Add(richting);
                    }
                }
            }
            return niveaus;
        }   
        catch (Exception ex)
        {
            throw new System.Data.DataException("Kon studierichtingen niet ophalen.", ex);
        }
    }
}
