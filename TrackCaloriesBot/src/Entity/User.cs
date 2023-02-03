using System.ComponentModel.DataAnnotations;
using TrackCaloriesBot.Enums;

namespace TrackCaloriesBot.Entity;

public class User
{
    public User()
    {
    }

    [Key] 
    public long TgId { get; set; }
    public string? Username { get; set; }
    public string? Name { get; set; }
    public int Age { get; set; }
    public Gender? Gender { get; set; }
    public float Weight { get; set; }
    public float Height { get; set; }
    public Goal? Goal { get; set; }
    public float GoalWeight { get; set; }
    public float ProjectedProgress { get; set; }
    public ActivityLevel? ActivityLevel { get; set; }
    public int RegistrationStage { get; set; }
    public ICollection<DayTotalData> DayTotalData { get; set; }
}