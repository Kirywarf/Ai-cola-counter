using CalorieCounter.Models;

namespace CalorieCounter.Services;

public class FoodEntryService
{
    private readonly DatabaseService _database;

    public FoodEntryService(DatabaseService database)
    {
        _database = database;
    }

    public List<FoodEntry> GetByDate(int profileId, DateTime date)
    {
        using var connection = _database.CreateConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM FoodEntries WHERE ProfileId=$profile AND Date=$date ORDER BY Id";
        cmd.Parameters.AddWithValue("$profile", profileId);
        cmd.Parameters.AddWithValue("$date", date.ToString("yyyy-MM-dd"));

        var result = new List<FoodEntry>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(Map(reader));
        }

        return result;
    }

    public List<DailySummary> GetSummaries(int profileId, int days)
    {
        using var connection = _database.CreateConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"SELECT Date,
SUM(Calories) AS Calories,
SUM(Protein) AS Protein,
SUM(Fat) AS Fat,
SUM(Carbs) AS Carbs
FROM FoodEntries
WHERE ProfileId=$profile AND Date >= $from
GROUP BY Date
ORDER BY Date";
        cmd.Parameters.AddWithValue("$profile", profileId);
        cmd.Parameters.AddWithValue("$from", DateTime.Today.AddDays(-(days - 1)).ToString("yyyy-MM-dd"));

        var list = new List<DailySummary>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new DailySummary
            {
                Date = DateTime.Parse(reader["Date"].ToString() ?? DateTime.Today.ToString("yyyy-MM-dd")),
                Calories = Convert.ToDouble(reader["Calories"]),
                Protein = Convert.ToDouble(reader["Protein"]),
                Fat = Convert.ToDouble(reader["Fat"]),
                Carbs = Convert.ToDouble(reader["Carbs"])
            });
        }

        return list;
    }

    public int Add(FoodEntry entry)
    {
        using var connection = _database.CreateConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO FoodEntries
(ProfileId, ProductId, Date, MealType, ProductName, WeightGrams, Calories, Protein, Fat, Carbs)
VALUES ($profile, $product, $date, $meal, $name, $grams, $cal, $pro, $fat, $carb);
SELECT last_insert_rowid();";
        Fill(cmd, entry);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Update(FoodEntry entry)
    {
        using var connection = _database.CreateConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = @"UPDATE FoodEntries SET
MealType=$meal, ProductName=$name, WeightGrams=$grams, Calories=$cal, Protein=$pro, Fat=$fat, Carbs=$carb
WHERE Id=$id";
        Fill(cmd, entry);
        cmd.Parameters.AddWithValue("$id", entry.Id);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = _database.CreateConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM FoodEntries WHERE Id=$id";
        cmd.Parameters.AddWithValue("$id", id);
        cmd.ExecuteNonQuery();
    }

    public void CopyToDate(int entryId, DateTime targetDate)
    {
        using var connection = _database.CreateConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO FoodEntries (ProfileId, ProductId, Date, MealType, ProductName, WeightGrams, Calories, Protein, Fat, Carbs)
SELECT ProfileId, ProductId, $date, MealType, ProductName, WeightGrams, Calories, Protein, Fat, Carbs
FROM FoodEntries WHERE Id=$id";
        cmd.Parameters.AddWithValue("$date", targetDate.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("$id", entryId);
        cmd.ExecuteNonQuery();
    }

    public void RepeatPreviousDay(int profileId, DateTime date)
    {
        var previousDay = date.AddDays(-1).ToString("yyyy-MM-dd");
        using var connection = _database.CreateConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO FoodEntries (ProfileId, ProductId, Date, MealType, ProductName, WeightGrams, Calories, Protein, Fat, Carbs)
SELECT ProfileId, ProductId, $targetDate, MealType, ProductName, WeightGrams, Calories, Protein, Fat, Carbs
FROM FoodEntries
WHERE ProfileId=$profile AND Date=$prevDate";
        cmd.Parameters.AddWithValue("$targetDate", date.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("$prevDate", previousDay);
        cmd.Parameters.AddWithValue("$profile", profileId);
        cmd.ExecuteNonQuery();
    }

    public void ClearDiary(int profileId)
    {
        using var connection = _database.CreateConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM FoodEntries WHERE ProfileId=$profile";
        cmd.Parameters.AddWithValue("$profile", profileId);
        cmd.ExecuteNonQuery();
    }

    private static FoodEntry Map(Microsoft.Data.Sqlite.SqliteDataReader reader) => new()
    {
        Id = Convert.ToInt32(reader["Id"]),
        ProfileId = Convert.ToInt32(reader["ProfileId"]),
        ProductId = reader["ProductId"] == DBNull.Value ? null : Convert.ToInt32(reader["ProductId"]),
        Date = DateTime.Parse(reader["Date"].ToString() ?? DateTime.Today.ToString("yyyy-MM-dd")),
        MealType = reader["MealType"].ToString() ?? "Завтрак",
        ProductName = reader["ProductName"].ToString() ?? string.Empty,
        WeightGrams = Convert.ToDouble(reader["WeightGrams"]),
        Calories = Convert.ToDouble(reader["Calories"]),
        Protein = Convert.ToDouble(reader["Protein"]),
        Fat = Convert.ToDouble(reader["Fat"]),
        Carbs = Convert.ToDouble(reader["Carbs"])
    };

    private static void Fill(Microsoft.Data.Sqlite.SqliteCommand cmd, FoodEntry entry)
    {
        cmd.Parameters.AddWithValue("$profile", entry.ProfileId);
        cmd.Parameters.AddWithValue("$product", entry.ProductId is null ? DBNull.Value : entry.ProductId);
        cmd.Parameters.AddWithValue("$date", entry.Date.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("$meal", entry.MealType);
        cmd.Parameters.AddWithValue("$name", entry.ProductName);
        cmd.Parameters.AddWithValue("$grams", entry.WeightGrams);
        cmd.Parameters.AddWithValue("$cal", entry.Calories);
        cmd.Parameters.AddWithValue("$pro", entry.Protein);
        cmd.Parameters.AddWithValue("$fat", entry.Fat);
        cmd.Parameters.AddWithValue("$carb", entry.Carbs);
    }
}
