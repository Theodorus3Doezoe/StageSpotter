using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using StageSpotter.Data.Interfaces;
using StageSpotter.Domain.Models;

namespace StageSpotter.Data.Repositories
{
    public class SavedVacatureRepository : ISavedVacatureRepository
    {
        private readonly string _connectionString;

        public SavedVacatureRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Create(SavedVacature savedVacature)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO SavedVacatures (UserId, VacatureId, CreatedAt) VALUES (@userId, @vacatureId, @createdAt); SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("@userId", savedVacature.UserId);
            command.Parameters.AddWithValue("@vacatureId", savedVacature.VacatureId);
            command.Parameters.AddWithValue("@createdAt", savedVacature.CreatedAt.ToString("o"));

            var result = command.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public IEnumerable<SavedVacature> GetByUserId(int userId)
        {
            var list = new List<SavedVacature>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, UserId, VacatureId, CreatedAt FROM SavedVacatures WHERE UserId = @userId ORDER BY CreatedAt DESC;";
            command.Parameters.AddWithValue("@userId", userId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var sv = new SavedVacature
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    VacatureId = reader.GetInt32(2),
                    CreatedAt = DateTime.Parse(reader.GetString(3))
                };
                list.Add(sv);
            }

            return list;
        }

        public int Delete(int userId, int vacatureId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM SavedVacatures WHERE UserId = @userId AND VacatureId = @vacatureId;";
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@vacatureId", vacatureId);

            var rows = command.ExecuteNonQuery();
            return rows;
        }
    }
}
