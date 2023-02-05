using System.ComponentModel.DataAnnotations;
using TrackCaloriesBot.Enums;

namespace TrackCaloriesBot.Entity;

public class MealData
{
    public MealData()
    {
    }

    [Key] public Guid MealId { get; set; }
    public MealType MealType { get; set; }
    public DayTotalData? DayTotalData { get; set; }
    public ICollection<Product>? Products { get; set; }
}