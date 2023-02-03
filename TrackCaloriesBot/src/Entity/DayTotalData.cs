using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;

namespace TrackCaloriesBot.Entity;

public class DayTotalData
{
    public DayTotalData()
    {
    }
    
    [Key] public long dayId { get; set; }
    public long tgId { get; set; }
    public MealData MealData { get; set; }
    public int Water { get; set; }
    public JSType.Date Date { get; set; }
    
}