using System.Collections.ObjectModel;
using System.Windows;
using CalorieCounter.Helpers;
using CalorieCounter.Models;
using CalorieCounter.Services;

namespace CalorieCounter.ViewModels;

public class ProfileSelectionViewModel : BaseViewModel
{
    private readonly MainViewModel _mainViewModel;
    private readonly ProfileService _profileService;
    private readonly NutritionCalculationService _nutritionService;
    private UserProfile? _selectedProfile;

    public ProfileSelectionViewModel(MainViewModel mainViewModel, ProfileService profileService, NutritionCalculationService nutritionService)
    {
        _mainViewModel = mainViewModel;
        _profileService = profileService;
        _nutritionService = nutritionService;

        CreateCommand = new RelayCommand(_ => CreateProfile());
        EditCommand = new RelayCommand(_ => EditProfile(), _ => SelectedProfile is not null);
        DeleteCommand = new RelayCommand(_ => DeleteProfile(), _ => SelectedProfile is not null);
        SelectCommand = new RelayCommand(_ => SelectProfile(), _ => SelectedProfile is not null);

        Refresh();
    }

    public ObservableCollection<UserProfile> Profiles { get; } = [];

    public UserProfile? SelectedProfile
    {
        get => _selectedProfile;
        set => SetProperty(ref _selectedProfile, value);
    }

    public RelayCommand CreateCommand { get; }
    public RelayCommand EditCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public RelayCommand SelectCommand { get; }

    private void Refresh()
    {
        Profiles.Clear();
        foreach (var p in _profileService.GetAll())
        {
            Profiles.Add(p);
        }
    }

    private void CreateProfile()
    {
        var vm = new ProfileEditViewModel(_profileService, _nutritionService, null, Refresh);
        var view = new Views.ProfileEditView { DataContext = vm, Owner = Application.Current.MainWindow };
        view.ShowDialog();
    }

    private void EditProfile()
    {
        var vm = new ProfileEditViewModel(_profileService, _nutritionService, SelectedProfile, Refresh);
        var view = new Views.ProfileEditView { DataContext = vm, Owner = Application.Current.MainWindow };
        view.ShowDialog();
    }

    private void DeleteProfile()
    {
        if (SelectedProfile is null)
        {
            return;
        }

        var result = MessageBox.Show($"Удалить профиль '{SelectedProfile.Name}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        _profileService.Delete(SelectedProfile.Id);
        Refresh();
        _mainViewModel.LoadProfiles();
    }

    private void SelectProfile()
    {
        if (SelectedProfile is null)
        {
            return;
        }

        _mainViewModel.LoadProfiles();
        _mainViewModel.SelectedProfile = _mainViewModel.Profiles.FirstOrDefault(p => p.Id == SelectedProfile.Id);
        _mainViewModel.ShowDashboard();
    }
}
