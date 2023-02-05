using System.ComponentModel.DataAnnotations;
using TrackCaloriesBot.Enums;

namespace TrackCaloriesBot.Entity;

public class Product
{
    public Product()
    {
    }

    [Key] public long ProductId { get; set; }
    public string Name { get; set; }
    public ServingType ServingType { get; set; }
    public int ServingAmount { get; set; }
    public int Quantity { get; set; }
    public int BaseCalories { get; set; }
    public int BaseProtein { get; set; }
    public int BaseFat { get; set; }
    public int BaseCarbs { get; set; }
    public MealData MealData { get; set; }
}