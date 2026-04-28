using CalorieCounter.Models;

namespace CalorieCounter.Services;

public class NutritionCalculationService
{
    private static readonly Dictionary<string, double> ActivityMultipliers = new()
    {
        ["Низкая активность"] = 1.2,
        ["Лёгкая активность"] = 1.375,
        ["Средняя активность"] = 1.55,
        ["Высокая активность"] = 1.725
    };

    public NutritionGoal CalculateGoal(UserProfile profile)
    {
        var bmr = profile.Gender == "Женский"
            ? 10 * profile.Weight + 6.25 * profile.Height - 5 * profile.Age - 161
            : 10 * profile.Weight + 6.25 * profile.Height - 5 * profile.Age + 5;

        var activity = ActivityMultipliers.GetValueOrDefault(profile.ActivityLevel, 1.55);
        var tdee = bmr * activity;

        var adjusted = profile.Goal switch
        {
            "Похудение" => tdee * 0.85,
            "Набор массы" => tdee * 1.10,
            _ => tdee
        };

        var protein = adjusted * 0.30 / 4;
        var fat = adjusted * 0.30 / 9;
        var carbs = adjusted * 0.40 / 4;

        return new NutritionGoal
        {
            DailyCaloriesGoal = Math.Round(adjusted, 0),
            DailyProteinGoal = Math.Round(protein, 1),
            DailyFatGoal = Math.Round(fat, 1),
            DailyCarbsGoal = Math.Round(carbs, 1)
        };
    }

    public FoodEntry CalculateFromProduct(int profileId, FoodProduct product, double grams, string mealType, DateTime date)
    {
        var factor = grams / 100.0;
        return new FoodEntry
        {
            ProfileId = profileId,
            ProductId = product.Id,
            ProductName = product.Name,
            Date = date.Date,
            MealType = mealType,
            WeightGrams = grams,
            Calories = Math.Round(product.CaloriesPer100g * factor, 1),
            Protein = Math.Round(product.ProteinPer100g * factor, 1),
            Fat = Math.Round(product.FatPer100g * factor, 1),
            Carbs = Math.Round(product.CarbsPer100g * factor, 1)
        };
    }
}
