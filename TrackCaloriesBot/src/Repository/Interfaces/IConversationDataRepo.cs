using Telegram.Bot.Types;
using TrackCaloriesBot.Entity;

namespace TrackCaloriesBot.Repository.Interfaces;

public interface IConversationDataRepo
{
    public ConversationData? CreateAddProductConversation(Update update);
    public ConversationData? GetAddProductConversation(long tgId);
    public void IncrementStage(long id);
    public void DecrementStage(long id);
    public void AddCommandName(Update update);
    public void AddProductId(Update update, long id);
    public void DeleteConversation(ConversationData conversation);
}