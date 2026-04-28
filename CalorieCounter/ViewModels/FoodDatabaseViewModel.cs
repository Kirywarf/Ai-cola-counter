using System.Collections.ObjectModel;
using System.Windows;
using CalorieCounter.Helpers;
using CalorieCounter.Models;
using CalorieCounter.Services;

namespace CalorieCounter.ViewModels;

public class FoodDatabaseViewModel : BaseViewModel
{
    private readonly FoodProductService _service;
    private FoodProduct? _selectedProduct;

    public FoodDatabaseViewModel(FoodProductService service)
    {
        _service = service;
        Categories = ["Все", "Крупы", "Мясо", "Рыба", "Молочные продукты", "Овощи", "Фрукты", "Напитки", "Сладости", "Фастфуд", "Готовые блюда", "Другое"];
        SelectedCategory = Categories[0];
        EditableProduct = new FoodProduct();

        SearchCommand = new RelayCommand(_ => Load());
        NewCommand = new RelayCommand(_ =>
        {
            EditableProduct = new FoodProduct { Category = "Другое", IsUserCreated = true };
            RaisePropertyChanged(nameof(EditableProduct));
        });
        SaveCommand = new RelayCommand(_ => Save());
        DeleteCommand = new RelayCommand(_ => Delete(), _ => SelectedProduct is not null);

        Load();
    }

    public ObservableCollection<FoodProduct> Products { get; } = [];
    public List<string> Categories { get; }
    public string? SearchText { get; set; }
    public string SelectedCategory { get; set; }

    public FoodProduct? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            if (SetProperty(ref _selectedProduct, value) && value is not null)
            {
                EditableProduct = new FoodProduct
                {
                    Id = value.Id,
                    Name = value.Name,
                    Category = value.Category,
                    CaloriesPer100g = value.CaloriesPer100g,
                    ProteinPer100g = value.ProteinPer100g,
                    FatPer100g = value.FatPer100g,
                    CarbsPer100g = value.CarbsPer100g,
                    IsUserCreated = value.IsUserCreated
                };
                RaisePropertyChanged(nameof(EditableProduct));
            }
        }
    }

    public FoodProduct EditableProduct { get; set; }

    public RelayCommand SearchCommand { get; }
    public RelayCommand NewCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand DeleteCommand { get; }

    private void Load()
    {
        Products.Clear();
        foreach (var p in _service.GetAll(SearchText, SelectedCategory))
        {
            Products.Add(p);
        }
    }

    private void Save()
    {
        if (string.IsNullOrWhiteSpace(EditableProduct.Name))
        {
            MessageBox.Show("Имя продукта не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (EditableProduct.CaloriesPer100g < 0 || EditableProduct.ProteinPer100g < 0 || EditableProduct.FatPer100g < 0 || EditableProduct.CarbsPer100g < 0)
        {
            MessageBox.Show("Пищевая ценность не может быть отрицательной.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (EditableProduct.Id == 0)
        {
            EditableProduct.IsUserCreated = true;
            _service.Add(EditableProduct);
        }
        else
        {
            _service.Update(EditableProduct);
        }

        Load();
        MessageBox.Show("Продукт сохранён.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void Delete()
    {
        if (SelectedProduct is null)
        {
            return;
        }

        if (MessageBox.Show("Удалить продукт?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
        {
            return;
        }

        _service.Delete(SelectedProduct.Id);
        Load();
    }
}
