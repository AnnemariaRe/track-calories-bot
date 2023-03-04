using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackCaloriesBot.Entity;

public class DayTotalData
{
    public DayTotalData()
    {
    }
    
    [Key] public int DayId { get; set; }
    public ICollection<MealData>? MealData { get; set; }
    public float Water { get; set; }
    [Index(IsUnique=true)] public string Date { get; set; }
    public long UserId { get; set; }
}