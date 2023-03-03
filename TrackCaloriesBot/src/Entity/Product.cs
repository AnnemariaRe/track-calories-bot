using System.ComponentModel.DataAnnotations;
using TrackCaloriesBot.Enums;

namespace TrackCaloriesBot.Entity;

public class Product
{
    public Product()
    {
    }

    [Key] public int Id { get; set; }
    public string? Name { get; set; }
    public string? ServingType { get; set; }
    public double ServingAmount { get; set; }
    public int Quantity { get; set; }
    public double BaseCalories { get; set; }
    public double BaseProtein { get; set; }
    public double BaseFat { get; set; }
    public double BaseCarbs { get; set; }
    public MealData? MealData { get; set; }
}