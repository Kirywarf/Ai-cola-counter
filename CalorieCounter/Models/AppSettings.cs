namespace CalorieCounter.Models;

public class AppSettings
{
    public int Id { get; set; } = 1;
    public string Theme { get; set; } = "Тёмная";
    public int? LastSelectedProfileId { get; set; }
}
