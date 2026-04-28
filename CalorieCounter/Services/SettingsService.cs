using CalorieCounter.Models;

namespace CalorieCounter.Services;

public class SettingsService
{
    private readonly DatabaseService _database;

    public SettingsService(DatabaseService database)
    {
        _database = database;
    }

    public AppSettings Get()
    {
        using var connection = _database.CreateConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM Settings WHERE Id=1";
        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
        {
            return new AppSettings();
        }

        return new AppSettings
        {
            Id = 1,
            Theme = reader["Theme"].ToString() ?? "Тёмная",
            LastSelectedProfileId = reader["LastSelectedProfileId"] == DBNull.Value ? null : Convert.ToInt32(reader["LastSelectedProfileId"])
        };
    }

    public void Save(AppSettings settings)
    {
        using var connection = _database.CreateConnection();
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "UPDATE Settings SET Theme=$theme, LastSelectedProfileId=$profile WHERE Id=1";
        cmd.Parameters.AddWithValue("$theme", settings.Theme);
        cmd.Parameters.AddWithValue("$profile", settings.LastSelectedProfileId is null ? DBNull.Value : settings.LastSelectedProfileId);
        cmd.ExecuteNonQuery();
    }
}
