using CalorieCounter.Models;

namespace CalorieCounter.Services;

public class StatisticsService
{
    private readonly FoodEntryService _entryService;

    public StatisticsService(FoodEntryService entryService)
    {
        _entryService = entryService;
    }

    public (List<DailySummary> Last7Days, List<DailySummary> Last30Days) GetCaloriesStats(int profileId)
    {
        return (_entryService.GetSummaries(profileId, 7), _entryService.GetSummaries(profileId, 30));
    }

    public double GetAverageCalories(int profileId)
    {
        var list = _entryService.GetSummaries(profileId, 30);
        return list.Count == 0 ? 0 : Math.Round(list.Average(x => x.Calories), 1);
    }

    public int CountDaysOverGoal(int profileId, double goal)
    {
        return _entryService.GetSummaries(profileId, 30).Count(x => x.Calories > goal);
    }

    public int CountDaysUnderGoal(int profileId, double goal)
    {
        return _entryService.GetSummaries(profileId, 30).Count(x => x.Calories < goal);
    }
}
