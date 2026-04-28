using CalorieCounter.Data;
using CalorieCounter.Models;
using Microsoft.Data.Sqlite;

namespace CalorieCounter.Services;

public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(string databasePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);
        _connectionString = $"Data Source={databasePath}";
    }

    public SqliteConnection CreateConnection() => new(_connectionString);

    public void InitializeDatabase()
    {
        using var connection = CreateConnection();
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
CREATE TABLE IF NOT EXISTS Profiles (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Age INTEGER NOT NULL,
    Height REAL NOT NULL,
    Weight REAL NOT NULL,
    Gender TEXT NOT NULL,
    Goal TEXT NOT NULL,
    ActivityLevel TEXT NOT NULL,
    DailyCaloriesGoal REAL NOT NULL,
    DailyProteinGoal REAL NOT NULL,
    DailyFatGoal REAL NOT NULL,
    DailyCarbsGoal REAL NOT NULL
);
CREATE TABLE IF NOT EXISTS FoodProducts (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Category TEXT NOT NULL,
    CaloriesPer100g REAL NOT NULL,
    ProteinPer100g REAL NOT NULL,
    FatPer100g REAL NOT NULL,
    CarbsPer100g REAL NOT NULL,
    IsUserCreated INTEGER NOT NULL DEFAULT 0
);
CREATE TABLE IF NOT EXISTS FoodEntries (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ProfileId INTEGER NOT NULL,
    ProductId INTEGER NULL,
    Date TEXT NOT NULL,
    MealType TEXT NOT NULL,
    ProductName TEXT NOT NULL,
    WeightGrams REAL NOT NULL,
    Calories REAL NOT NULL,
    Protein REAL NOT NULL,
    Fat REAL NOT NULL,
    Carbs REAL NOT NULL,
    FOREIGN KEY(ProfileId) REFERENCES Profiles(Id)
);
CREATE TABLE IF NOT EXISTS Settings (
    Id INTEGER PRIMARY KEY,
    Theme TEXT NOT NULL,
    LastSelectedProfileId INTEGER NULL
);";
        command.ExecuteNonQuery();

        SeedProducts(connection);
        EnsureDefaultSettings(connection);
    }

    private static void EnsureDefaultSettings(SqliteConnection connection)
    {
        var existsCmd = connection.CreateCommand();
        existsCmd.CommandText = "SELECT COUNT(*) FROM Settings WHERE Id=1";
        var exists = Convert.ToInt32(existsCmd.ExecuteScalar());
        if (exists == 0)
        {
            var insert = connection.CreateCommand();
            insert.CommandText = "INSERT INTO Settings (Id, Theme, LastSelectedProfileId) VALUES (1, 'Тёмная', NULL)";
            insert.ExecuteNonQuery();
        }
    }

    private static void SeedProducts(SqliteConnection connection)
    {
        var countCmd = connection.CreateCommand();
        countCmd.CommandText = "SELECT COUNT(*) FROM FoodProducts WHERE IsUserCreated = 0";
        var count = Convert.ToInt32(countCmd.ExecuteScalar());
        if (count > 0)
        {
            return;
        }

        var items = ProductSeedData.GetSeedProducts();
        foreach (var p in items)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO FoodProducts
(Name, Category, CaloriesPer100g, ProteinPer100g, FatPer100g, CarbsPer100g, IsUserCreated)
VALUES ($name, $cat, $cal, $pro, $fat, $carb, 0)";
            cmd.Parameters.AddWithValue("$name", p.Name);
            cmd.Parameters.AddWithValue("$cat", p.Category);
            cmd.Parameters.AddWithValue("$cal", p.CaloriesPer100g);
            cmd.Parameters.AddWithValue("$pro", p.ProteinPer100g);
            cmd.Parameters.AddWithValue("$fat", p.FatPer100g);
            cmd.Parameters.AddWithValue("$carb", p.CarbsPer100g);
            cmd.ExecuteNonQuery();
        }
    }
}
