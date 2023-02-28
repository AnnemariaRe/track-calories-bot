using System.ComponentModel.DataAnnotations;

namespace TrackCaloriesBot.Entity.Requests;

public class RequestRecipe
{
    [Required]
    [Key]
    public long UserId { get; set; }
    public string Equipments { get; set; }
    public string Ingredients { get; set; }
    public int MaxReadyTime { get; set; }
}