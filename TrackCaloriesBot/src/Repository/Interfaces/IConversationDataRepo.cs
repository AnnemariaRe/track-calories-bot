using Telegram.Bot.Types;
using TrackCaloriesBot.Entity;

namespace TrackCaloriesBot.Repository.Interfaces;

public interface IConversationDataRepo
{
    public ConversationData? CreateConversation(Update update);
    public ConversationData? GetConversationData(long tgId);
    public void IncrementStage(long id);
    public void IncrementStageBy(long id, int amount);
    public void DecrementStage(long id);
    public void DecrementStageBy(long id, int amount);
    public void AddCommandName(string? data, long id);
    public void AddItemId(long chatId, int id);
    public void AddRecipeId(long chatId, int id);
    public void AddLastMessageId(long chatId, int id);
    public void DeleteConversation(ConversationData conversation);
}