using System.ComponentModel.DataAnnotations;


namespace TrackCaloriesBot.Entity;

public class DayTotalData
{
    public DayTotalData()
    {
    }
    
    [Key] public int dayId { get; set; }
    public long tgId { get; set; }
    public ICollection<MealData>? MealData { get; set; }
    public float Water { get; set; }
    public DateTime Date { get; set; }
}