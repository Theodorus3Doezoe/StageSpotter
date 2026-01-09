using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using StageSpotter.Data.Interfaces;
using StageSpotter.Data.DTOs;

namespace StageSpotter.Data.Repositories;

public class OpleidingsniveauRepository : IOpleidingsniveauRepository
{
    private readonly string _connectionString;

    public OpleidingsniveauRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    // Maak de koppeling tussen vacature en opleidingsniveau 
    public void AddVacatureNiveau(int vacatureId, int niveauId)
    {
        try
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                string sqlQuery =
                    "INSERT INTO VacatureOpleidingsniveaus (VacatureId, OpleidingsniveauId) VALUES (@VacatureId, @OpleidingsniveauId)";
                
                SqliteCommand command = new SqliteCommand(sqlQuery, connection);
                
                command.Parameters.AddWithValue("@VacatureId", vacatureId);
                command.Parameters.AddWithValue("@OpleidingsniveauId", niveauId);
                
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            throw new System.Data.DataException("Kon vacature niveau niet toevoegen aan vacature.", ex);
        }
    }
    
    public List<OpleidingsniveauDto> GetAlleOpleidingsniveaus() 
    {
        try
        {
            List<OpleidingsniveauDto> niveaus = new List<OpleidingsniveauDto>();
        
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                string sqlQuery = "SELECT Id, Niveau FROM Opleidingsniveaus ORDER BY Niveau"; 
            
                SqliteCommand command = new SqliteCommand(sqlQuery, connection);
            
                connection.Open();
            
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var niveau = new OpleidingsniveauDto
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Niveau = reader["Niveau"].ToString() 
                        };
                        niveaus.Add(niveau);
                    }
                }
            } 
            return niveaus; 
        }   
        catch (Exception ex)
        {
            throw new System.Data.DataException("Kon opleidingsniveaus niet ophalen.", ex);
        }
    }
}

