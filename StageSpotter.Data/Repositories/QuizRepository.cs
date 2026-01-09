using Microsoft.Data.Sqlite;
using StageSpotter.Data.Interfaces;
using StageSpotter.Domain.Models;

namespace StageSpotter.Data.Repositories
{
    public class QuizRepository : IQuizRepository
    {
        private readonly string _connectionString;

        public QuizRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void SavePreferences(UserPreference prefs)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var checkCmd = connection.CreateCommand();
            checkCmd.CommandText = "SELECT Id FROM UserPreferences WHERE UserId = @userId LIMIT 1;";
            checkCmd.Parameters.AddWithValue("@userId", prefs.UserId);
            var existing = checkCmd.ExecuteScalar();

            if (existing != null)
            {
                using var update = connection.CreateCommand();
                update.CommandText = "UPDATE UserPreferences SET Werkstijl = @werkstijl, Bedrijfstype = @bedrijfstype, Focus = @focus, Leerdoel = @leerdoel WHERE UserId = @userId;";
                update.Parameters.AddWithValue("@werkstijl", prefs.Werkstijl ?? string.Empty);
                update.Parameters.AddWithValue("@bedrijfstype", prefs.Bedrijfstype ?? string.Empty);
                update.Parameters.AddWithValue("@focus", prefs.Focus ?? string.Empty);
                update.Parameters.AddWithValue("@leerdoel", prefs.Leerdoel ?? string.Empty);
                update.Parameters.AddWithValue("@userId", prefs.UserId);
                update.ExecuteNonQuery();
            }
            else
            {
                using var insert = connection.CreateCommand();
                insert.CommandText = "INSERT INTO UserPreferences (UserId, Werkstijl, Bedrijfstype, Focus, Leerdoel) VALUES (@userId, @werkstijl, @bedrijfstype, @focus, @leerdoel);";
                insert.Parameters.AddWithValue("@userId", prefs.UserId);
                insert.Parameters.AddWithValue("@werkstijl", prefs.Werkstijl ?? string.Empty);
                insert.Parameters.AddWithValue("@bedrijfstype", prefs.Bedrijfstype ?? string.Empty);
                insert.Parameters.AddWithValue("@focus", prefs.Focus ?? string.Empty);
                insert.Parameters.AddWithValue("@leerdoel", prefs.Leerdoel ?? string.Empty);
                insert.ExecuteNonQuery();
            }
        }

        public UserPreference GetPreferencesByUserId(int userId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Id, UserId, Werkstijl, Bedrijfstype, Focus, Leerdoel FROM UserPreferences WHERE UserId = @userId LIMIT 1;";
            cmd.Parameters.AddWithValue("@userId", userId);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            var prefs = new UserPreference();
            prefs.Id = reader.GetInt32(0);
            prefs.UserId = reader.GetInt32(1);
            prefs.Werkstijl = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
            prefs.Bedrijfstype = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
            prefs.Focus = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
            prefs.Leerdoel = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
            return prefs;
        }
    }
}
