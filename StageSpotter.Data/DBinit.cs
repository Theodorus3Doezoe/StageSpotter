using Microsoft.Data.Sqlite;

namespace StageSpotter.Data
{
    public static class DatabaseInitializer
    {
        public static void Initialize(string connectionString)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    -- Bedrijven
                    CREATE TABLE IF NOT EXISTS Bedrijven (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Naam TEXT NOT NULL,
                        BedrijfUrl TEXT
                    );

                    -- Vacatures
                    CREATE TABLE IF NOT EXISTS Vacatures (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Titel TEXT NOT NULL,
                        Beschrijving TEXT,
                        Locatie TEXT,
                        BedrijfId INTEGER NOT NULL,
                        PublicatieDatum TEXT NOT NULL,
                        IsActief INTEGER DEFAULT 1,
                        SoortStage INTEGER NOT NULL,
                        VacatureUrl TEXT,
                        FOREIGN KEY(BedrijfId) REFERENCES Bedrijven(Id)
                    );

                    -- Opleidingsniveaus
                    CREATE TABLE IF NOT EXISTS Opleidingsniveaus (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Niveau TEXT NOT NULL
                    );

                    -- Studierichtingen
                    CREATE TABLE IF NOT EXISTS Studierichtingen (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Richting TEXT NOT NULL
                    );

                    -- Koppeltabel Vacature <-> Opleidingsniveau
                    CREATE TABLE IF NOT EXISTS VacatureOpleidingsniveaus (
                        VacatureId INTEGER NOT NULL,
                        OpleidingsniveauId INTEGER NOT NULL,
                        PRIMARY KEY (VacatureId, OpleidingsniveauId),
                        FOREIGN KEY(VacatureId) REFERENCES Vacatures(Id),
                        FOREIGN KEY(OpleidingsniveauId) REFERENCES Opleidingsniveaus(Id)
                    );

                    -- Koppeltabel Vacature <-> Studierichting
                    CREATE TABLE IF NOT EXISTS VacatureStudierichtingen (
                        VacatureId INTEGER NOT NULL,
                        StudierichtingId INTEGER NOT NULL,
                        PRIMARY KEY (VacatureId, StudierichtingId),
                        FOREIGN KEY(VacatureId) REFERENCES Vacatures(Id),
                        FOREIGN KEY(StudierichtingId) REFERENCES Studierichtingen(Id)
                    );

                    -- Voeg eventueel dummy data toe als tabel leeg is (optioneel)
                    INSERT OR IGNORE INTO Bedrijven (Id, Naam, BedrijfUrl) VALUES (1, 'Test Bedrijf', 'www.test.nl');
                    INSERT OR IGNORE INTO Opleidingsniveaus (Id, Niveau) VALUES (1, 'HBO'), (2, 'WO'), (3, 'MBO');
                    INSERT OR IGNORE INTO Studierichtingen (Id, Richting) VALUES (1, 'ICT'), (2, 'Marketing');
                ";

                command.ExecuteNonQuery();
            }
        }
    }
}
