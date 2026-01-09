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

                // Drop dependent tables if they exist to ensure fresh schema
                try
                {
                    using (var dropCommand = connection.CreateCommand())
                    {
                        dropCommand.CommandText = "DROP TABLE IF EXISTS SavedMotivationLetters;";
                        dropCommand.ExecuteNonQuery();
                    }
                    using (var dropCommand = connection.CreateCommand())
                    {
                        dropCommand.CommandText = "DROP TABLE IF EXISTS SavedVacatures;";
                        dropCommand.ExecuteNonQuery();
                    }
                    using (var dropCommand = connection.CreateCommand())
                    {
                        dropCommand.CommandText = "DROP TABLE IF EXISTS SavedAnalyses;";
                        dropCommand.ExecuteNonQuery();
                    }
                    using (var dropCommand = connection.CreateCommand())
                    {
                        dropCommand.CommandText = "DROP TABLE IF EXISTS UserPreferences;";
                        dropCommand.ExecuteNonQuery();
                    }
                    using (var dropCommand = connection.CreateCommand())
                    {
                        dropCommand.CommandText = "DROP TABLE IF EXISTS Users;";
                        dropCommand.ExecuteNonQuery();
                    }
                }
                catch { }

                var command = connection.CreateCommand();
                command.CommandText = @"
                    -- Bedrijven
                    CREATE TABLE IF NOT EXISTS Bedrijven (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Naam TEXT NOT NULL,
                        BedrijfUrl TEXT,
                        KvKNummer TEXT,
                        ContactPerson TEXT,
                        ContactEmail TEXT
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
                    
                    -- Users
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Email TEXT NOT NULL UNIQUE,
                        PasswordHash TEXT NOT NULL,
                        Type INTEGER DEFAULT 0,
                        BedrijfId INTEGER,
                        FOREIGN KEY(BedrijfId) REFERENCES Bedrijven(Id)
                    );
                    
                    -- Saved analyses per user
                    CREATE TABLE IF NOT EXISTS SavedAnalyses (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER NOT NULL,
                        FileName TEXT NOT NULL,
                        Result TEXT NOT NULL,
                        CreatedAt TEXT NOT NULL,
                        FOREIGN KEY(UserId) REFERENCES Users(Id)
                    );

                    -- Saved vacancies per user
                    CREATE TABLE IF NOT EXISTS SavedVacatures (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER NOT NULL,
                        VacatureId INTEGER NOT NULL,
                        CreatedAt TEXT NOT NULL,
                        FOREIGN KEY(UserId) REFERENCES Users(Id),
                        FOREIGN KEY(VacatureId) REFERENCES Vacatures(Id)
                    );

                    -- Reviews for companies
                    CREATE TABLE IF NOT EXISTS Reviews (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER NOT NULL,
                        BedrijfId INTEGER NOT NULL,
                        Title TEXT NOT NULL,
                        Description TEXT,
                        Rating INTEGER NOT NULL,
                        CreatedAt TEXT NOT NULL,
                        FOREIGN KEY(UserId) REFERENCES Users(Id),
                        FOREIGN KEY(BedrijfId) REFERENCES Bedrijven(Id)
                    );

                    -- User preferences (quiz)
                    CREATE TABLE IF NOT EXISTS UserPreferences (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER NOT NULL UNIQUE,
                        Werkstijl TEXT,
                        Bedrijfstype TEXT,
                        Focus TEXT,
                        Leerdoel TEXT,
                        FOREIGN KEY(UserId) REFERENCES Users(Id)
                    );

                    -- Saved motivation letters
                    CREATE TABLE IF NOT EXISTS SavedMotivationLetters (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER NOT NULL,
                        VacatureId INTEGER NOT NULL,
                        Content TEXT NOT NULL,
                        VacatureTitel TEXT,
                        BedrijfNaam TEXT,
                        CreatedAt TEXT NOT NULL,
                        FOREIGN KEY(UserId) REFERENCES Users(Id),
                        FOREIGN KEY(VacatureId) REFERENCES Vacatures(Id)
                    );
                ";

                command.ExecuteNonQuery();
            }
        }
    }
}

