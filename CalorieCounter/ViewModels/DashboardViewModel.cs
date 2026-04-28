using CalorieCounter.Helpers;
using CalorieCounter.Models;
using CalorieCounter.Services;

namespace CalorieCounter.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    private readonly UserProfile _profile;
    private readonly FoodEntryService _entryService;

    public DashboardViewModel(UserProfile profile, FoodEntryService entryService)
    {
        _profile = profile;
        _entryService = entryService;
        Today = DateTime.Today;
        Refresh();
    }

    public DateTime Today { get; }
    public string ProfileName => _profile.Name;
    public double DailyGoal => _profile.DailyCaloriesGoal;
    public double ConsumedCalories { get; private set; }
    public double RemainingCalories => Math.Round(DailyGoal - ConsumedCalories, 1);
    public double Protein { get; private set; }
    public double Fat { get; private set; }
    public double Carbs { get; private set; }
    public double Progress => DailyGoal <= 0 ? 0 : Math.Min(100, ConsumedCalories / DailyGoal * 100);

    public void Refresh()
    {
        var entries = _entryService.GetByDate(_profile.Id, DateTime.Today);
        ConsumedCalories = Math.Round(entries.Sum(x => x.Calories), 1);
        Protein = Math.Round(entries.Sum(x => x.Protein), 1);
        Fat = Math.Round(entries.Sum(x => x.Fat), 1);
        Carbs = Math.Round(entries.Sum(x => x.Carbs), 1);
        RaisePropertyChanged(nameof(ConsumedCalories));
        RaisePropertyChanged(nameof(RemainingCalories));
        RaisePropertyChanged(nameof(Protein));
        RaisePropertyChanged(nameof(Fat));
        RaisePropertyChanged(nameof(Carbs));
        RaisePropertyChanged(nameof(Progress));
    }
}
