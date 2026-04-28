using System.Text;
using CalorieCounter.Models;

namespace CalorieCounter.Services;

public class CsvExportService
{
    public void Export(string filePath, string profileName, IEnumerable<FoodEntry> entries)
    {
        var sb = new StringBuilder();
        sb.AppendLine("дата,профиль,приём пищи,продукт,вес,калории,белки,жиры,углеводы");
        foreach (var entry in entries.OrderBy(e => e.Date).ThenBy(e => e.MealType))
        {
            sb.AppendLine(string.Join(',',
                entry.Date.ToString("yyyy-MM-dd"),
                Escape(profileName),
                Escape(entry.MealType),
                Escape(entry.ProductName),
                entry.WeightGrams.ToString("0.##"),
                entry.Calories.ToString("0.##"),
                entry.Protein.ToString("0.##"),
                entry.Fat.ToString("0.##"),
                entry.Carbs.ToString("0.##")));
        }

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
    }

    private static string Escape(string value)
    {
        return value.Contains(',') ? $"\"{value.Replace("\"", "\"\"")}\"" : value;
    }
}
