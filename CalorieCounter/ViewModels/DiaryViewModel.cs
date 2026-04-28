using System.Collections.ObjectModel;
using System.Windows;
using CalorieCounter.Helpers;
using CalorieCounter.Models;
using CalorieCounter.Services;

namespace CalorieCounter.ViewModels;

public class DiaryViewModel : BaseViewModel
{
    private readonly UserProfile _profile;
    private readonly FoodEntryService _service;
    private FoodEntry? _selectedEntry;

    public DiaryViewModel(UserProfile profile, FoodEntryService service)
    {
        _profile = profile;
        _service = service;
        SelectedDate = DateTime.Today;

        LoadCommand = new RelayCommand(_ => Load());
        DeleteCommand = new RelayCommand(_ => Delete(), _ => SelectedEntry is not null);
        SaveCommand = new RelayCommand(_ => Save(), _ => SelectedEntry is not null);
        CopyCommand = new RelayCommand(_ => CopyEntry(), _ => SelectedEntry is not null);
        RepeatYesterdayCommand = new RelayCommand(_ => RepeatYesterday());

        Load();
    }

    public DateTime SelectedDate { get; set; }
    public ObservableCollection<FoodEntry> Entries { get; } = [];
    public List<string> MealTypes { get; } = ["Завтрак", "Обед", "Ужин", "Перекус"];
    public DateTime CopyTargetDate { get; set; } = DateTime.Today;

    public FoodEntry? SelectedEntry
    {
        get => _selectedEntry;
        set => SetProperty(ref _selectedEntry, value);
    }

    public double TotalCalories => Entries.Sum(x => x.Calories);
    public double TotalProtein => Entries.Sum(x => x.Protein);
    public double TotalFat => Entries.Sum(x => x.Fat);
    public double TotalCarbs => Entries.Sum(x => x.Carbs);

    public RelayCommand LoadCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand CopyCommand { get; }
    public RelayCommand RepeatYesterdayCommand { get; }

    private void Load()
    {
        Entries.Clear();
        foreach (var e in _service.GetByDate(_profile.Id, SelectedDate))
        {
            Entries.Add(e);
        }

        RaisePropertyChanged(nameof(TotalCalories));
        RaisePropertyChanged(nameof(TotalProtein));
        RaisePropertyChanged(nameof(TotalFat));
        RaisePropertyChanged(nameof(TotalCarbs));
    }

    private void Save()
    {
        if (SelectedEntry is null)
        {
            return;
        }

        if (SelectedEntry.WeightGrams <= 0 || SelectedEntry.Calories < 0 || SelectedEntry.Protein < 0 || SelectedEntry.Fat < 0 || SelectedEntry.Carbs < 0)
        {
            MessageBox.Show("Проверьте значения записи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _service.Update(SelectedEntry);
        Load();
    }

    private void Delete()
    {
        if (SelectedEntry is null)
        {
            return;
        }

        _service.Delete(SelectedEntry.Id);
        Load();
    }

    private void CopyEntry()
    {
        if (SelectedEntry is null)
        {
            return;
        }

        _service.CopyToDate(SelectedEntry.Id, CopyTargetDate);
        MessageBox.Show("Запись скопирована.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void RepeatYesterday()
    {
        _service.RepeatPreviousDay(_profile.Id, SelectedDate);
        Load();
    }
}
