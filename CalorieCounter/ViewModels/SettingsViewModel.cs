using System.Windows;
using CalorieCounter.Helpers;
using CalorieCounter.Models;
using CalorieCounter.Services;
using Microsoft.Win32;

namespace CalorieCounter.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly UserProfile _profile;
    private readonly SettingsService _settingsService;
    private readonly FoodEntryService _entryService;
    private readonly FoodProductService _productService;
    private readonly CsvExportService _csvExportService;

    public SettingsViewModel(UserProfile profile, SettingsService settingsService, FoodEntryService entryService, FoodProductService productService, CsvExportService csvExportService)
    {
        _profile = profile;
        _settingsService = settingsService;
        _entryService = entryService;
        _productService = productService;
        _csvExportService = csvExportService;

        ThemeOptions = ["Тёмная", "Светлая"];
        var settings = _settingsService.Get();
        SelectedTheme = settings.Theme;

        CaloriesGoal = _profile.DailyCaloriesGoal;

        SaveSettingsCommand = new RelayCommand(_ => SaveSettings());
        ExportCommand = new RelayCommand(_ => Export());
        ClearDiaryCommand = new RelayCommand(_ => ClearDiary());
        ClearUserProductsCommand = new RelayCommand(_ => ClearUserProducts());
    }

    public List<string> ThemeOptions { get; }
    public string SelectedTheme { get; set; }
    public double CaloriesGoal { get; set; }

    public RelayCommand SaveSettingsCommand { get; }
    public RelayCommand ExportCommand { get; }
    public RelayCommand ClearDiaryCommand { get; }
    public RelayCommand ClearUserProductsCommand { get; }

    private void SaveSettings()
    {
        if (CaloriesGoal < 0)
        {
            MessageBox.Show("Норма калорий не может быть отрицательной.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _profile.DailyCaloriesGoal = CaloriesGoal;
        var settings = _settingsService.Get();
        settings.Theme = SelectedTheme;
        _settingsService.Save(settings);

        MessageBox.Show("Настройки сохранены. Перезапустите приложение для полного применения темы.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void Export()
    {
        var entries = _entryService.GetSummaries(_profile.Id, 365).SelectMany(s => _entryService.GetByDate(_profile.Id, s.Date)).ToList();
        if (entries.Count == 0)
        {
            MessageBox.Show("Нет данных для экспорта.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var dialog = new SaveFileDialog
        {
            Filter = "CSV (*.csv)|*.csv",
            FileName = $"diary_{_profile.Name}_{DateTime.Today:yyyyMMdd}.csv"
        };

        if (dialog.ShowDialog() == true)
        {
            _csvExportService.Export(dialog.FileName, _profile.Name, entries);
            MessageBox.Show("Экспорт завершён.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void ClearDiary()
    {
        if (MessageBox.Show("Очистить весь дневник текущего профиля?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            _entryService.ClearDiary(_profile.Id);
        }
    }

    private void ClearUserProducts()
    {
        if (MessageBox.Show("Удалить пользовательские продукты?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            _productService.DeleteUserProducts();
        }
    }
}
