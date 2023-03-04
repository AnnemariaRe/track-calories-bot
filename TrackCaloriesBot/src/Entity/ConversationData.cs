using System.ComponentModel.DataAnnotations;

namespace TrackCaloriesBot.Entity;

[Serializable]
public class ConversationData
{
    [Required]
    [Key]
    public long UserId { get; set; }
    public string? CommandName { get; set; }
    public string? MealType { get; set; }
    public int ItemId { get; set; }
    public int RecipeId { get; set; }
    public int ConversationStage { get; set; }
}