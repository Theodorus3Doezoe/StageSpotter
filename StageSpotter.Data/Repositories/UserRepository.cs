using Microsoft.Data.Sqlite;
using StageSpotter.Data.Interfaces;
using StageSpotter.Domain.Models;

namespace StageSpotter.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int CreateUser(User user)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Users (Email, PasswordHash, Type, BedrijfId) VALUES (@email, @passwordHash, @type, @bedrijfId); SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@type", (int)user.Type);
            command.Parameters.AddWithValue("@bedrijfId", (object?)user.BedrijfId ?? DBNull.Value);

            var result = command.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public User GetUserByEmail(string email)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Email, PasswordHash, Type, BedrijfId FROM Users WHERE Email = @email LIMIT 1;";
            command.Parameters.AddWithValue("@email", email);

            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            var user = new User();
            user.Id = reader.GetInt32(0);
            user.Email = reader.GetString(1);
            user.PasswordHash = reader.GetString(2);
            user.Type = (StageSpotter.Domain.Models.UserType)reader.GetInt32(3);
            if (!reader.IsDBNull(4)) user.BedrijfId = reader.GetInt32(4);
            return user;
        }
    }
}
