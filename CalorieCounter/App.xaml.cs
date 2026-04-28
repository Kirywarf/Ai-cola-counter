using System.Windows;
using System.Windows.Media;
using CalorieCounter.Services;

namespace CalorieCounter;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CalorieCounter", "calorie_counter.db");
        var db = new DatabaseService(dbPath);
        db.InitializeDatabase();
        var settings = new SettingsService(db).Get();
        ApplyTheme(settings.Theme);
    }

    public void ApplyTheme(string theme)
    {
        var dark = theme != "Светлая";
        Resources["BgBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(dark ? "#1B1C22" : "#F2F4F7"));
        Resources["PanelBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(dark ? "#2A2C35" : "#FFFFFF"));
        Resources["ForegroundBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(dark ? "#F2F2F2" : "#1E1E1E"));
        Resources["InputBgBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(dark ? "#31333E" : "#FFFFFF"));
    }
}
