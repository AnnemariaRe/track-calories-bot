using System.ComponentModel.DataAnnotations;

namespace TrackCaloriesBot.Entity;

public class AddProductConversation
{
    [Key] public Guid Id { get; set; }
    public User User { get; set; }
    public string? CommandName { get; set; }
    public string MealType { get; set; }
    public int ConversationStage { get; set; }
}