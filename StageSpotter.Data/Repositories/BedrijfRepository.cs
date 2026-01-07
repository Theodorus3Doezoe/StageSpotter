using Microsoft.Extensions.Configuration;
using StageSpotter.Data.Interfaces;
using StageSpotter.Data.DTOs;

            // TODO Errors terugkoppelen naar frontend
            
namespace StageSpotter.Data.Repositories;
using Microsoft.Data.Sqlite;


public class BedrijfRepository : IBedrijfRepository
{
    private readonly string _connectionString;

    public BedrijfRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
    }

    //Controleren of een bedrijf al bestaat voor de vacature toevoegen
    public BedrijfDto? FindByName(string bedrijfsnaam)
    {
        try
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                string sqlQuery = "SELECT * FROM Bedrijven WHERE Naam = @bedrijfsnaam";

                SqliteCommand command = new SqliteCommand(sqlQuery, connection);
                
                command.Parameters.AddWithValue("@bedrijfsnaam", bedrijfsnaam);
                connection.Open();
                
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new BedrijfDto
                        {
                            Id = (int)reader["Id"],
                            Naam = reader["Naam"].ToString(),
                            BedrijfUrl = reader["BedrijfUrl"].ToString()
                        };
                    }
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            throw new System.Data.DataException("Kon bedrijf niet ophalen.", ex);
        }

    }
    
    // Bedrijf aanmaken
    public BedrijfDto Create(BedrijfDto bedrijfToCreate)
    {
        try
        {
            using (SqliteConnection connection = new SqliteConnection(_connectionString))
            {
                // output inserted.id 
                string sqlQuery = "INSERT INTO Bedrijven (Naam, BedrijfUrl) VALUES (@naam, @bedrijfUrl); SELECT last_insert_rowid()";    
                SqliteCommand command = new SqliteCommand(sqlQuery, connection);
                
                command.Parameters.AddWithValue("@naam",  bedrijfToCreate.Naam);
                command.Parameters.AddWithValue("@bedrijfUrl",  bedrijfToCreate.BedrijfUrl);

                connection.Open();
                
                var newId = Convert.ToInt32(command.ExecuteScalar());
                return new BedrijfDto
                {
                    Id = newId,
                    Naam = bedrijfToCreate.Naam,
                    BedrijfUrl = bedrijfToCreate.BedrijfUrl
                };
            }
        }
        catch (Exception ex)
        {
            throw new System.Data.DataException("Kon bedrijf niet aanmaken.", ex);
        }
    }
}
