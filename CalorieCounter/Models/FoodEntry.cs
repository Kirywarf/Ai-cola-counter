namespace CalorieCounter.Models;

public class FoodEntry
{
    public int Id { get; set; }
    public int ProfileId { get; set; }
    public int? ProductId { get; set; }
    public DateTime Date { get; set; }
    public string MealType { get; set; } = "Завтрак";
    public string ProductName { get; set; } = string.Empty;
    public double WeightGrams { get; set; }
    public double Calories { get; set; }
    public double Protein { get; set; }
    public double Fat { get; set; }
    public double Carbs { get; set; }
}
