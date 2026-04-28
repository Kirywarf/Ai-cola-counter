using System.Collections.ObjectModel;
using System.Windows;
using CalorieCounter.Helpers;
using CalorieCounter.Models;
using CalorieCounter.Services;

namespace CalorieCounter.ViewModels;

public class AddFoodViewModel : BaseViewModel
{
    private readonly UserProfile _profile;
    private readonly FoodProductService _productService;
    private readonly FoodEntryService _entryService;
    private readonly NutritionCalculationService _nutrition;

    private FoodProduct? _selectedProduct;
    private double _weight = 100;

    public AddFoodViewModel(UserProfile profile, FoodProductService productService, FoodEntryService entryService, NutritionCalculationService nutrition)
    {
        _profile = profile;
        _productService = productService;
        _entryService = entryService;
        _nutrition = nutrition;

        MealTypes = ["Завтрак", "Обед", "Ужин", "Перекус"];
        SelectedMealType = MealTypes[0];

        AddCommand = new RelayCommand(_ => AddSelectedProduct());
        AddManualCommand = new RelayCommand(_ => AddManual());

        LoadProducts();
    }

    public ObservableCollection<FoodProduct> Products { get; } = [];
    public List<string> MealTypes { get; }
    public string SelectedMealType { get; set; }

    public FoodProduct? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            if (SetProperty(ref _selectedProduct, value))
            {
                RaisePropertyChanged(nameof(PreviewCalories));
                RaisePropertyChanged(nameof(PreviewProtein));
                RaisePropertyChanged(nameof(PreviewFat));
                RaisePropertyChanged(nameof(PreviewCarbs));
            }
        }
    }

    public double Weight
    {
        get => _weight;
        set
        {
            if (SetProperty(ref _weight, value))
            {
                RaisePropertyChanged(nameof(PreviewCalories));
                RaisePropertyChanged(nameof(PreviewProtein));
                RaisePropertyChanged(nameof(PreviewFat));
                RaisePropertyChanged(nameof(PreviewCarbs));
            }
        }
    }

    public string ManualName { get; set; } = string.Empty;
    public double ManualWeight { get; set; } = 100;
    public double ManualCalories { get; set; }
    public double ManualProtein { get; set; }
    public double ManualFat { get; set; }
    public double ManualCarbs { get; set; }
    public bool SaveManualToDatabase { get; set; }

    public double PreviewCalories => SelectedProduct is null ? 0 : Math.Round(SelectedProduct.CaloriesPer100g * Weight / 100.0, 1);
    public double PreviewProtein => SelectedProduct is null ? 0 : Math.Round(SelectedProduct.ProteinPer100g * Weight / 100.0, 1);
    public double PreviewFat => SelectedProduct is null ? 0 : Math.Round(SelectedProduct.FatPer100g * Weight / 100.0, 1);
    public double PreviewCarbs => SelectedProduct is null ? 0 : Math.Round(SelectedProduct.CarbsPer100g * Weight / 100.0, 1);

    public RelayCommand AddCommand { get; }
    public RelayCommand AddManualCommand { get; }

    private void LoadProducts()
    {
        Products.Clear();
        foreach (var p in _productService.GetAll())
        {
            Products.Add(p);
        }
    }

    private void AddSelectedProduct()
    {
        if (SelectedProduct is null)
        {
            MessageBox.Show("Выберите продукт.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (Weight <= 0)
        {
            MessageBox.Show("Вес должен быть больше нуля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var entry = _nutrition.CalculateFromProduct(_profile.Id, SelectedProduct, Weight, SelectedMealType, DateTime.Today);
        _entryService.Add(entry);
        MessageBox.Show("Еда добавлена в дневник.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void AddManual()
    {
        if (string.IsNullOrWhiteSpace(ManualName))
        {
            MessageBox.Show("Имя продукта не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (ManualWeight <= 0 || ManualCalories < 0 || ManualProtein < 0 || ManualFat < 0 || ManualCarbs < 0)
        {
            MessageBox.Show("Проверьте корректность значений ручного ввода.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var entry = new FoodEntry
        {
            ProfileId = _profile.Id,
            Date = DateTime.Today,
            MealType = SelectedMealType,
            ProductName = ManualName,
            WeightGrams = ManualWeight,
            Calories = ManualCalories,
            Protein = ManualProtein,
            Fat = ManualFat,
            Carbs = ManualCarbs
        };

        _entryService.Add(entry);

        if (SaveManualToDatabase)
        {
            var factor = 100.0 / ManualWeight;
            _productService.Add(new FoodProduct
            {
                Name = ManualName,
                Category = "Другое",
                CaloriesPer100g = Math.Round(ManualCalories * factor, 1),
                ProteinPer100g = Math.Round(ManualProtein * factor, 1),
                FatPer100g = Math.Round(ManualFat * factor, 1),
                CarbsPer100g = Math.Round(ManualCarbs * factor, 1),
                IsUserCreated = true
            });
            LoadProducts();
        }

        MessageBox.Show("Ручная запись добавлена.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
