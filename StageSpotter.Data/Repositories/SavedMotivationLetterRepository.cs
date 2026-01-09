using Microsoft.Data.Sqlite;

namespace StageSpotter.Data.Repositories
{
    public class SavedMotivationLetterRepository : StageSpotter.Data.Interfaces.ISavedMotivationLetterRepository
    {
        private readonly string _connectionString;

        public SavedMotivationLetterRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SaveMotivationLetter(int userId, int vacatureId, string content, string vacatureTitel, string bedrijfNaam)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO SavedMotivationLetters (UserId, VacatureId, Content, VacatureTitel, BedrijfNaam, CreatedAt)
                    VALUES (@userId, @vacatureId, @content, @vacatureTitel, @bedrijfNaam, @createdAt)
                ";
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@vacatureId", vacatureId);
                command.Parameters.AddWithValue("@content", content);
                command.Parameters.AddWithValue("@vacatureTitel", vacatureTitel ?? string.Empty);
                command.Parameters.AddWithValue("@bedrijfNaam", bedrijfNaam ?? string.Empty);
                command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow.ToString("o"));
                command.ExecuteNonQuery();
            }
        }

        public List<dynamic> GetByUserId(int userId)
        {
            var result = new List<dynamic>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT Id, VacatureId, Content, VacatureTitel, BedrijfNaam, CreatedAt
                    FROM SavedMotivationLetters
                    WHERE UserId = @userId
                    ORDER BY CreatedAt DESC
                ";
                command.Parameters.AddWithValue("@userId", userId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new
                        {
                            Id = (int)(long)reader["Id"],
                            VacatureId = (int)(long)reader["VacatureId"],
                            Content = (string)reader["Content"],
                            VacatureTitel = (string)reader["VacatureTitel"],
                            BedrijfNaam = (string)reader["BedrijfNaam"],
                            CreatedAt = (string)reader["CreatedAt"]
                        });
                    }
                }
            }
            return result;
        }

        public void DeleteById(int id)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM SavedMotivationLetters WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
        }
    }
}
