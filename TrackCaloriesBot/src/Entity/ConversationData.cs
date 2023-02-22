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
    public long? ProductId { get; set; }
    public int ConversationStage { get; set; }
}