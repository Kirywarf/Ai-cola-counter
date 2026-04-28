using System.Windows;
using CalorieCounter.Helpers;
using CalorieCounter.Models;
using CalorieCounter.Services;

namespace CalorieCounter.ViewModels;

public class ProfileEditViewModel : BaseViewModel
{
    private readonly ProfileService _profileService;
    private readonly NutritionCalculationService _nutritionService;
    private readonly Action _onSaved;
    private readonly UserProfile? _existing;

    public ProfileEditViewModel(ProfileService profileService, NutritionCalculationService nutritionService, UserProfile? existing, Action onSaved)
    {
        _profileService = profileService;
        _nutritionService = nutritionService;
        _onSaved = onSaved;
        _existing = existing;

        GenderOptions = ["Мужской", "Женский"];
        GoalOptions = ["Похудение", "Поддержание веса", "Набор массы"];
        ActivityOptions = ["Низкая активность", "Лёгкая активность", "Средняя активность", "Высокая активность"];

        if (existing is not null)
        {
            Profile = new UserProfile
            {
                Id = existing.Id,
                Name = existing.Name,
                Age = existing.Age,
                Height = existing.Height,
                Weight = existing.Weight,
                Gender = existing.Gender,
                Goal = existing.Goal,
                ActivityLevel = existing.ActivityLevel,
                DailyCaloriesGoal = existing.DailyCaloriesGoal,
                DailyProteinGoal = existing.DailyProteinGoal,
                DailyFatGoal = existing.DailyFatGoal,
                DailyCarbsGoal = existing.DailyCarbsGoal
            };
        }

        RecalculateCommand = new RelayCommand(_ => Recalculate());
        SaveCommand = new RelayCommand(SaveProfile);
    }

    public UserProfile Profile { get; private set; } = new();
    public List<string> GenderOptions { get; }
    public List<string> GoalOptions { get; }
    public List<string> ActivityOptions { get; }

    public RelayCommand RecalculateCommand { get; }
    public RelayCommand SaveCommand { get; }

    private void Recalculate()
    {
        if (!Validate())
        {
            return;
        }

        var goal = _nutritionService.CalculateGoal(Profile);
        Profile.DailyCaloriesGoal = goal.DailyCaloriesGoal;
        Profile.DailyProteinGoal = goal.DailyProteinGoal;
        Profile.DailyFatGoal = goal.DailyFatGoal;
        Profile.DailyCarbsGoal = goal.DailyCarbsGoal;
        RaisePropertyChanged(nameof(Profile));
    }

    private void SaveProfile(object? parameter)
    {
        if (!Validate())
        {
            return;
        }

        if (_existing is null)
        {
            _profileService.Add(Profile);
        }
        else
        {
            _profileService.Update(Profile);
        }

        _onSaved();
        if (parameter is Window window)
        {
            window.DialogResult = true;
            window.Close();
        }
    }

    private bool Validate()
    {
        if (string.IsNullOrWhiteSpace(Profile.Name))
        {
            MessageBox.Show("Имя профиля не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (Profile.Age <= 0 || Profile.Height <= 0 || Profile.Weight <= 0)
        {
            MessageBox.Show("Возраст, рост и вес должны быть положительными числами.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (Profile.DailyCaloriesGoal < 0 || Profile.DailyProteinGoal < 0 || Profile.DailyFatGoal < 0 || Profile.DailyCarbsGoal < 0)
        {
            MessageBox.Show("Нормы не могут быть отрицательными.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        return true;
    }
}
