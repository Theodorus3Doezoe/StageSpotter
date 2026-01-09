using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using StageSpotter.Data.Interfaces;
using StageSpotter.Domain.Models;

namespace StageSpotter.Data.Repositories
{
    public class SavedAnalysisRepository : ISavedAnalysisRepository
    {
        private readonly string _connectionString;

        public SavedAnalysisRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Create(SavedAnalysis analysis)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO SavedAnalyses (UserId, FileName, Result, CreatedAt) VALUES (@userId, @fileName, @result, @createdAt); SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("@userId", analysis.UserId);
            command.Parameters.AddWithValue("@fileName", analysis.FileName ?? string.Empty);
            command.Parameters.AddWithValue("@result", analysis.Result ?? string.Empty);
            command.Parameters.AddWithValue("@createdAt", analysis.CreatedAt.ToString("o"));

            var result = command.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public IEnumerable<SavedAnalysis> GetByUserId(int userId)
        {
            var list = new List<SavedAnalysis>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, UserId, FileName, Result, CreatedAt FROM SavedAnalyses WHERE UserId = @userId ORDER BY CreatedAt DESC;";
            command.Parameters.AddWithValue("@userId", userId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var sa = new SavedAnalysis
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    FileName = reader.GetString(2),
                    Result = reader.GetString(3),
                    CreatedAt = DateTime.Parse(reader.GetString(4))
                };
                list.Add(sa);
            }

            return list;
        }

        public SavedAnalysis? GetById(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, UserId, FileName, Result, CreatedAt FROM SavedAnalyses WHERE Id = @id LIMIT 1;";
            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new SavedAnalysis
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    FileName = reader.GetString(2),
                    Result = reader.GetString(3),
                    CreatedAt = DateTime.Parse(reader.GetString(4))
                };
            }

            return null;
        }
    }
}
