using System.Collections.ObjectModel;
using System.Windows;
using CalorieCounter.Helpers;
using CalorieCounter.Models;
using CalorieCounter.Services;

namespace CalorieCounter.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly DatabaseService _database;
    private readonly ProfileService _profileService;
    private readonly FoodProductService _productService;
    private readonly FoodEntryService _entryService;
    private readonly NutritionCalculationService _nutritionService;
    private readonly StatisticsService _statisticsService;
    private readonly CsvExportService _csvExportService;
    private readonly SettingsService _settingsService;

    private BaseViewModel? _currentViewModel;
    private UserProfile? _selectedProfile;

    public MainViewModel()
    {
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CalorieCounter", "calorie_counter.db");
        _database = new DatabaseService(dbPath);
        _database.InitializeDatabase();

        _profileService = new ProfileService(_database);
        _productService = new FoodProductService(_database);
        _entryService = new FoodEntryService(_database);
        _nutritionService = new NutritionCalculationService();
        _statisticsService = new StatisticsService(_entryService);
        _csvExportService = new CsvExportService();
        _settingsService = new SettingsService(_database);

        NavigateCommand = new RelayCommand(Navigate);

        LoadProfiles();
        if (Profiles.Count == 0)
        {
            ShowProfileSelection();
        }
        else
        {
            var settings = _settingsService.Get();
            SelectedProfile = settings.LastSelectedProfileId is int id
                ? Profiles.FirstOrDefault(x => x.Id == id) ?? Profiles.First()
                : Profiles.First();
            ShowDashboard();
        }
    }

    public ObservableCollection<UserProfile> Profiles { get; } = [];

    public UserProfile? SelectedProfile
    {
        get => _selectedProfile;
        set
        {
            if (SetProperty(ref _selectedProfile, value) && value is not null)
            {
                var settings = _settingsService.Get();
                settings.LastSelectedProfileId = value.Id;
                _settingsService.Save(settings);
                RaisePropertyChanged(nameof(ProfileHeader));
            }
        }
    }

    public string ProfileHeader => SelectedProfile is null ? "Профиль не выбран" : $"Профиль: {SelectedProfile.Name}";

    public BaseViewModel? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public RelayCommand NavigateCommand { get; }

    public void LoadProfiles()
    {
        Profiles.Clear();
        foreach (var p in _profileService.GetAll())
        {
            Profiles.Add(p);
        }
    }

    public void ShowProfileSelection()
    {
        CurrentViewModel = new ProfileSelectionViewModel(this, _profileService, _nutritionService);
    }

    public void ShowDashboard()
    {
        if (SelectedProfile is null)
        {
            MessageBox.Show("Сначала выберите профиль.", "Calorie Counter", MessageBoxButton.OK, MessageBoxImage.Information);
            ShowProfileSelection();
            return;
        }

        CurrentViewModel = new DashboardViewModel(SelectedProfile, _entryService);
    }

    private void Navigate(object? parameter)
    {
        if (parameter is not string section)
        {
            return;
        }

        if (section != "Профили" && SelectedProfile is null)
        {
            MessageBox.Show("Сначала создайте или выберите профиль.", "Calorie Counter", MessageBoxButton.OK, MessageBoxImage.Warning);
            ShowProfileSelection();
            return;
        }

        CurrentViewModel = section switch
        {
            "Профили" => new ProfileSelectionViewModel(this, _profileService, _nutritionService),
            "Главная" => new DashboardViewModel(SelectedProfile!, _entryService),
            "Добавить еду" => new AddFoodViewModel(SelectedProfile!, _productService, _entryService, _nutritionService),
            "Дневник" => new DiaryViewModel(SelectedProfile!, _entryService),
            "База продуктов" => new FoodDatabaseViewModel(_productService),
            "Статистика" => new StatisticsViewModel(SelectedProfile!, _entryService, _statisticsService),
            "Настройки" => new SettingsViewModel(SelectedProfile!, _settingsService, _entryService, _productService, _csvExportService),
            _ => CurrentViewModel!
        };
    }
}
