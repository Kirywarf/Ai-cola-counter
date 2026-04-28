using CalorieCounter.Models;

namespace CalorieCounter.Services;

public class ProfileService
{
    private readonly DatabaseService _database;

    public ProfileService(DatabaseService database)
    {
        _database = database;
    }

    public List<UserProfile> GetAll()
    {
        var result = new List<UserProfile>();
        using var connection = _database.CreateConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM Profiles ORDER BY Name";
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(ReadProfile(reader));
        }

        return result;
    }

    public UserProfile? GetById(int id)
    {
        using var connection = _database.CreateConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM Profiles WHERE Id=$id";
        cmd.Parameters.AddWithValue("$id", id);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? ReadProfile(reader) : null;
    }

    public int Add(UserProfile profile)
    {
        using var connection = _database.CreateConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO Profiles
(Name, Age, Height, Weight, Gender, Goal, ActivityLevel, DailyCaloriesGoal, DailyProteinGoal, DailyFatGoal, DailyCarbsGoal)
VALUES
($name,$age,$height,$weight,$gender,$goal,$activity,$cal,$pro,$fat,$carb);
SELECT last_insert_rowid();";
        FillParameters(cmd, profile);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Update(UserProfile profile)
    {
        using var connection = _database.CreateConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = @"UPDATE Profiles SET
Name=$name, Age=$age, Height=$height, Weight=$weight, Gender=$gender, Goal=$goal, ActivityLevel=$activity,
DailyCaloriesGoal=$cal, DailyProteinGoal=$pro, DailyFatGoal=$fat, DailyCarbsGoal=$carb
WHERE Id=$id";
        FillParameters(cmd, profile);
        cmd.Parameters.AddWithValue("$id", profile.Id);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = _database.CreateConnection();
        connection.Open();

        var deleteEntries = connection.CreateCommand();
        deleteEntries.CommandText = "DELETE FROM FoodEntries WHERE ProfileId=$id";
        deleteEntries.Parameters.AddWithValue("$id", id);
        deleteEntries.ExecuteNonQuery();

        var deleteProfile = connection.CreateCommand();
        deleteProfile.CommandText = "DELETE FROM Profiles WHERE Id=$id";
        deleteProfile.Parameters.AddWithValue("$id", id);
        deleteProfile.ExecuteNonQuery();
    }

    private static UserProfile ReadProfile(Microsoft.Data.Sqlite.SqliteDataReader reader) => new()
    {
        Id = Convert.ToInt32(reader["Id"]),
        Name = reader["Name"].ToString() ?? string.Empty,
        Age = Convert.ToInt32(reader["Age"]),
        Height = Convert.ToDouble(reader["Height"]),
        Weight = Convert.ToDouble(reader["Weight"]),
        Gender = reader["Gender"].ToString() ?? "Мужской",
        Goal = reader["Goal"].ToString() ?? "Поддержание веса",
        ActivityLevel = reader["ActivityLevel"].ToString() ?? "Средняя активность",
        DailyCaloriesGoal = Convert.ToDouble(reader["DailyCaloriesGoal"]),
        DailyProteinGoal = Convert.ToDouble(reader["DailyProteinGoal"]),
        DailyFatGoal = Convert.ToDouble(reader["DailyFatGoal"]),
        DailyCarbsGoal = Convert.ToDouble(reader["DailyCarbsGoal"])
    };

    private static void FillParameters(Microsoft.Data.Sqlite.SqliteCommand cmd, UserProfile profile)
    {
        cmd.Parameters.AddWithValue("$name", profile.Name);
        cmd.Parameters.AddWithValue("$age", profile.Age);
        cmd.Parameters.AddWithValue("$height", profile.Height);
        cmd.Parameters.AddWithValue("$weight", profile.Weight);
        cmd.Parameters.AddWithValue("$gender", profile.Gender);
        cmd.Parameters.AddWithValue("$goal", profile.Goal);
        cmd.Parameters.AddWithValue("$activity", profile.ActivityLevel);
        cmd.Parameters.AddWithValue("$cal", profile.DailyCaloriesGoal);
        cmd.Parameters.AddWithValue("$pro", profile.DailyProteinGoal);
        cmd.Parameters.AddWithValue("$fat", profile.DailyFatGoal);
        cmd.Parameters.AddWithValue("$carb", profile.DailyCarbsGoal);
    }
}
