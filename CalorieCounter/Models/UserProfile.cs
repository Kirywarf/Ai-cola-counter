namespace CalorieCounter.Models;

public class UserProfile
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public double Height { get; set; }
    public double Weight { get; set; }
    public string Gender { get; set; } = "Мужской";
    public string Goal { get; set; } = "Поддержание веса";
    public string ActivityLevel { get; set; } = "Средняя активность";
    public double DailyCaloriesGoal { get; set; }
    public double DailyProteinGoal { get; set; }
    public double DailyFatGoal { get; set; }
    public double DailyCarbsGoal { get; set; }
}
