using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using StageSpotter.Data.Interfaces;
using StageSpotter.Domain.Models;

namespace StageSpotter.Data.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly string _connectionString;

        public ReviewRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Create(Review review)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Reviews (UserId, BedrijfId, Title, Description, Rating, CreatedAt) VALUES (@userId, @bedrijfId, @title, @description, @rating, @createdAt); SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("@userId", review.UserId);
            command.Parameters.AddWithValue("@bedrijfId", review.BedrijfId);
            command.Parameters.AddWithValue("@title", review.Title ?? string.Empty);
            command.Parameters.AddWithValue("@description", review.Description ?? string.Empty);
            command.Parameters.AddWithValue("@rating", review.Rating);
            command.Parameters.AddWithValue("@createdAt", review.CreatedAt.ToString("o"));

            var result = command.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public IEnumerable<Review> GetByBedrijfId(int bedrijfId)
        {
            var list = new List<Review>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, UserId, BedrijfId, Title, Description, Rating, CreatedAt FROM Reviews WHERE BedrijfId = @bedrijfId ORDER BY CreatedAt DESC;";
            command.Parameters.AddWithValue("@bedrijfId", bedrijfId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Review
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    BedrijfId = reader.GetInt32(2),
                    Title = reader.GetString(3),
                    Description = reader.GetString(4),
                    Rating = reader.GetInt32(5),
                    CreatedAt = DateTime.Parse(reader.GetString(6))
                });
            }

            return list;
        }

        public double GetAverageRating(int bedrijfId)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT AVG(Rating) FROM Reviews WHERE BedrijfId = @bedrijfId;";
            command.Parameters.AddWithValue("@bedrijfId", bedrijfId);

            var result = command.ExecuteScalar();
            if (result == null || result == DBNull.Value) return 0.0;
            return Convert.ToDouble(result);
        }
    }
}
