using CalorieCounter.Models;

namespace CalorieCounter.Services;

public class FoodProductService
{
    private readonly DatabaseService _database;

    public FoodProductService(DatabaseService database)
    {
        _database = database;
    }

    public List<FoodProduct> GetAll(string? search = null, string? category = null)
    {
        using var connection = _database.CreateConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"SELECT * FROM FoodProducts
WHERE ($search IS NULL OR Name LIKE '%' || $search || '%')
AND ($category IS NULL OR $category='' OR Category = $category)
ORDER BY Name";
        cmd.Parameters.AddWithValue("$search", string.IsNullOrWhiteSpace(search) ? DBNull.Value : search);
        cmd.Parameters.AddWithValue("$category", string.IsNullOrWhiteSpace(category) || category == "Все" ? DBNull.Value : category);

        var result = new List<FoodProduct>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new FoodProduct
            {
                Id = Convert.ToInt32(reader["Id"]),
                Name = reader["Name"].ToString() ?? string.Empty,
                Category = reader["Category"].ToString() ?? "Другое",
                CaloriesPer100g = Convert.ToDouble(reader["CaloriesPer100g"]),
                ProteinPer100g = Convert.ToDouble(reader["ProteinPer100g"]),
                FatPer100g = Convert.ToDouble(reader["FatPer100g"]),
                CarbsPer100g = Convert.ToDouble(reader["CarbsPer100g"]),
                IsUserCreated = Convert.ToInt32(reader["IsUserCreated"]) == 1
            });
        }

        return result;
    }

    public int Add(FoodProduct product)
    {
        using var connection = _database.CreateConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO FoodProducts
(Name, Category, CaloriesPer100g, ProteinPer100g, FatPer100g, CarbsPer100g, IsUserCreated)
VALUES ($name, $cat, $cal, $pro, $fat, $carb, $user);
SELECT last_insert_rowid();";
        Fill(cmd, product);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    public void Update(FoodProduct product)
    {
        using var connection = _database.CreateConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"UPDATE FoodProducts SET
Name=$name, Category=$cat, CaloriesPer100g=$cal, ProteinPer100g=$pro, FatPer100g=$fat, CarbsPer100g=$carb
WHERE Id=$id";
        Fill(cmd, product);
        cmd.Parameters.AddWithValue("$id", product.Id);
        cmd.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = _database.CreateConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM FoodProducts WHERE Id=$id";
        cmd.Parameters.AddWithValue("$id", id);
        cmd.ExecuteNonQuery();
    }

    public void DeleteUserProducts()
    {
        using var connection = _database.CreateConnection();
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM FoodProducts WHERE IsUserCreated=1";
        cmd.ExecuteNonQuery();
    }

    private static void Fill(Microsoft.Data.Sqlite.SqliteCommand cmd, FoodProduct product)
    {
        cmd.Parameters.AddWithValue("$name", product.Name);
        cmd.Parameters.AddWithValue("$cat", product.Category);
        cmd.Parameters.AddWithValue("$cal", product.CaloriesPer100g);
        cmd.Parameters.AddWithValue("$pro", product.ProteinPer100g);
        cmd.Parameters.AddWithValue("$fat", product.FatPer100g);
        cmd.Parameters.AddWithValue("$carb", product.CarbsPer100g);
        cmd.Parameters.AddWithValue("$user", product.IsUserCreated ? 1 : 0);
    }
}
