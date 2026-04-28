using System.Collections.ObjectModel;
using CalorieCounter.Helpers;
using CalorieCounter.Models;
using CalorieCounter.Services;

namespace CalorieCounter.ViewModels;

public class StatisticsViewModel : BaseViewModel
{
    public StatisticsViewModel(UserProfile profile, FoodEntryService entryService, StatisticsService statisticsService)
    {
        var last7 = entryService.GetSummaries(profile.Id, 7);
        var last30 = entryService.GetSummaries(profile.Id, 30);

        Last7DaysCalories = new ObservableCollection<string>(last7.Select(x => $"{x.Date:dd.MM}: {x.Calories:0}"));
        Last30DaysCalories = new ObservableCollection<string>(last30.Select(x => $"{x.Date:dd.MM}: {x.Calories:0}"));

        AverageCalories = statisticsService.GetAverageCalories(profile.Id);
        OverGoalDays = statisticsService.CountDaysOverGoal(profile.Id, profile.DailyCaloriesGoal);
        UnderGoalDays = statisticsService.CountDaysUnderGoal(profile.Id, profile.DailyCaloriesGoal);

        var allEntries = new List<FoodEntry>();
        foreach (var day in last30)
        {
            allEntries.AddRange(entryService.GetByDate(profile.Id, day.Date));
        }

        MostFrequentProduct = allEntries.GroupBy(x => x.ProductName).OrderByDescending(g => g.Count()).FirstOrDefault()?.Key ?? "Нет данных";
        MostCalorieDay = last30.OrderByDescending(x => x.Calories).FirstOrDefault()?.Date.ToString("dd.MM.yyyy") ?? "Нет данных";
    }

    public ObservableCollection<string> Last7DaysCalories { get; }
    public ObservableCollection<string> Last30DaysCalories { get; }
    public double AverageCalories { get; }
    public int OverGoalDays { get; }
    public int UnderGoalDays { get; }
    public string MostFrequentProduct { get; }
    public string MostCalorieDay { get; }
}
