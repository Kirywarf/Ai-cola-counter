using CalorieCounter.Models;

namespace CalorieCounter.Data;

public static class ProductSeedData
{
    public static List<FoodProduct> GetSeedProducts() =>
    [
        new() { Name="Гречка варёная", Category="Крупы", CaloriesPer100g=110, ProteinPer100g=4.2, FatPer100g=1.1, CarbsPer100g=21.3 },
        new() { Name="Рис варёный", Category="Крупы", CaloriesPer100g=116, ProteinPer100g=2.4, FatPer100g=0.3, CarbsPer100g=24.9 },
        new() { Name="Овсянка", Category="Крупы", CaloriesPer100g=88, ProteinPer100g=3.0, FatPer100g=1.7, CarbsPer100g=15.0 },
        new() { Name="Макароны варёные", Category="Крупы", CaloriesPer100g=131, ProteinPer100g=5.0, FatPer100g=1.1, CarbsPer100g=25.0 },
        new() { Name="Картофель варёный", Category="Овощи", CaloriesPer100g=82, ProteinPer100g=2.0, FatPer100g=0.4, CarbsPer100g=16.7 },
        new() { Name="Картофель жареный", Category="Фастфуд", CaloriesPer100g=192, ProteinPer100g=2.8, FatPer100g=9.5, CarbsPer100g=24.0 },
        new() { Name="Куриная грудка", Category="Мясо", CaloriesPer100g=165, ProteinPer100g=31.0, FatPer100g=3.6, CarbsPer100g=0.0 },
        new() { Name="Куриное бедро", Category="Мясо", CaloriesPer100g=209, ProteinPer100g=26.0, FatPer100g=10.9, CarbsPer100g=0.0 },
        new() { Name="Говядина", Category="Мясо", CaloriesPer100g=187, ProteinPer100g=18.9, FatPer100g=12.4, CarbsPer100g=0.0 },
        new() { Name="Свинина", Category="Мясо", CaloriesPer100g=242, ProteinPer100g=16.0, FatPer100g=21.6, CarbsPer100g=0.0 },
        new() { Name="Индейка", Category="Мясо", CaloriesPer100g=189, ProteinPer100g=29.0, FatPer100g=7.4, CarbsPer100g=0.0 },
        new() { Name="Лосось", Category="Рыба", CaloriesPer100g=208, ProteinPer100g=20.0, FatPer100g=13.0, CarbsPer100g=0.0 },
        new() { Name="Тунец", Category="Рыба", CaloriesPer100g=132, ProteinPer100g=28.0, FatPer100g=1.3, CarbsPer100g=0.0 },
        new() { Name="Минтай", Category="Рыба", CaloriesPer100g=72, ProteinPer100g=15.9, FatPer100g=0.9, CarbsPer100g=0.0 },
        new() { Name="Яйцо куриное", Category="Другое", CaloriesPer100g=157, ProteinPer100g=12.7, FatPer100g=11.5, CarbsPer100g=0.7 },
        new() { Name="Молоко 2.5%", Category="Молочные продукты", CaloriesPer100g=52, ProteinPer100g=2.8, FatPer100g=2.5, CarbsPer100g=4.7 },
        new() { Name="Творог 5%", Category="Молочные продукты", CaloriesPer100g=121, ProteinPer100g=17.2, FatPer100g=5.0, CarbsPer100g=2.8 },
        new() { Name="Йогурт натуральный", Category="Молочные продукты", CaloriesPer100g=66, ProteinPer100g=3.5, FatPer100g=3.2, CarbsPer100g=5.0 },
        new() { Name="Сыр твёрдый", Category="Молочные продукты", CaloriesPer100g=350, ProteinPer100g=25.0, FatPer100g=27.0, CarbsPer100g=0.0 },
        new() { Name="Масло сливочное", Category="Молочные продукты", CaloriesPer100g=748, ProteinPer100g=0.5, FatPer100g=82.5, CarbsPer100g=0.8 },
        new() { Name="Хлеб белый", Category="Другое", CaloriesPer100g=265, ProteinPer100g=8.0, FatPer100g=3.2, CarbsPer100g=49.0 },
        new() { Name="Хлеб чёрный", Category="Другое", CaloriesPer100g=214, ProteinPer100g=6.9, FatPer100g=1.3, CarbsPer100g=40.7 },
        new() { Name="Банан", Category="Фрукты", CaloriesPer100g=96, ProteinPer100g=1.5, FatPer100g=0.5, CarbsPer100g=21.0 },
        new() { Name="Яблоко", Category="Фрукты", CaloriesPer100g=47, ProteinPer100g=0.4, FatPer100g=0.4, CarbsPer100g=9.8 },
        new() { Name="Апельсин", Category="Фрукты", CaloriesPer100g=43, ProteinPer100g=0.9, FatPer100g=0.2, CarbsPer100g=8.1 },
        new() { Name="Помидор", Category="Овощи", CaloriesPer100g=20, ProteinPer100g=1.1, FatPer100g=0.2, CarbsPer100g=3.7 },
        new() { Name="Огурец", Category="Овощи", CaloriesPer100g=15, ProteinPer100g=0.8, FatPer100g=0.1, CarbsPer100g=2.8 },
        new() { Name="Морковь", Category="Овощи", CaloriesPer100g=35, ProteinPer100g=1.3, FatPer100g=0.1, CarbsPer100g=6.9 },
        new() { Name="Капуста", Category="Овощи", CaloriesPer100g=28, ProteinPer100g=1.8, FatPer100g=0.1, CarbsPer100g=4.7 },
        new() { Name="Салат овощной", Category="Готовые блюда", CaloriesPer100g=55, ProteinPer100g=1.5, FatPer100g=3.0, CarbsPer100g=5.2 },
        new() { Name="Борщ", Category="Готовые блюда", CaloriesPer100g=49, ProteinPer100g=2.0, FatPer100g=2.0, CarbsPer100g=6.0 },
        new() { Name="Суп куриный", Category="Готовые блюда", CaloriesPer100g=36, ProteinPer100g=2.9, FatPer100g=1.5, CarbsPer100g=2.8 },
        new() { Name="Пицца", Category="Фастфуд", CaloriesPer100g=266, ProteinPer100g=11.0, FatPer100g=10.0, CarbsPer100g=33.0 },
        new() { Name="Бургер", Category="Фастфуд", CaloriesPer100g=295, ProteinPer100g=15.0, FatPer100g=14.0, CarbsPer100g=28.0 },
        new() { Name="Шаурма", Category="Фастфуд", CaloriesPer100g=250, ProteinPer100g=10.0, FatPer100g=13.0, CarbsPer100g=24.0 },
        new() { Name="Картофель фри", Category="Фастфуд", CaloriesPer100g=312, ProteinPer100g=3.4, FatPer100g=15.0, CarbsPer100g=41.0 },
        new() { Name="Шоколад", Category="Сладости", CaloriesPer100g=535, ProteinPer100g=7.7, FatPer100g=30.0, CarbsPer100g=59.0 },
        new() { Name="Печенье", Category="Сладости", CaloriesPer100g=450, ProteinPer100g=6.0, FatPer100g=18.0, CarbsPer100g=65.0 },
        new() { Name="Кока-кола", Category="Напитки", CaloriesPer100g=42, ProteinPer100g=0.0, FatPer100g=0.0, CarbsPer100g=10.6 },
        new() { Name="Сок апельсиновый", Category="Напитки", CaloriesPer100g=45, ProteinPer100g=0.7, FatPer100g=0.2, CarbsPer100g=10.4 }
    ];
}
