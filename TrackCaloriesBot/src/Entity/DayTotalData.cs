using System.ComponentModel.DataAnnotations;

namespace TrackCaloriesBot.Entity;

public class DayTotalData
{
    public DayTotalData()
    {
    }
    
    [Key] public long dayId { get; set; }
    public long tgId { get; set; }
    public ICollection<MealData> MealData { get; set; }
    public int Water { get; set; }
    public DateTime Date { get; set; }
}