using System.Windows;
using CalorieCounter.ViewModels;

namespace CalorieCounter;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
