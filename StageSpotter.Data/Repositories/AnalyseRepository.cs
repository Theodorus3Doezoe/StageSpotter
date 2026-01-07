using System.Collections.Generic;
using Microsoft.Data.Sqlite; // AANGEPAST: Sqlite
using Microsoft.Extensions.Configuration;
using StageSpotter.Data.Interfaces;

namespace StageSpotter.Data.Repositories
{
    public class AnalyseRepository : IAnalyseRepository
    {
        private readonly string _connectionString;
        public AnalyseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'");
        }

        public List<(string CvBestandsnaam, string Resultaat, string AnalyseDatum)> GetAnalyses(int gebruikerId)
        {
            var result = new List<(string, string, string)>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = new SqliteCommand("SELECT CvBestandsnaam, Resultaat, AnalyseDatum FROM Analyses WHERE GebruikerId = @gebruikerId", conn);
            cmd.Parameters.AddWithValue("@gebruikerId", gebruikerId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add((reader.GetString(0), reader.GetString(1), reader.GetDateTime(2).ToString("yyyy-MM-dd HH:mm")));
            }
            return result;
        }
    }
}
