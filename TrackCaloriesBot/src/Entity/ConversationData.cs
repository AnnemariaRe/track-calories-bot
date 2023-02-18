namespace TrackCaloriesBot.Entity;

public class ConversationData
{
    public long UserId { get; set; }
    public string? CommandName { get; set; }
    public string? MealType { get; set; }
    public long? ProductId { get; set; }
    public int ConversationStage { get; set; }
}