using Telegram.Bot.Types;
using TrackCaloriesBot.Entity;

namespace TrackCaloriesBot.Repository.Interfaces;

public interface IConversationDataRepo
{
    public Task<ConversationData> CreateAddProductConversation(Update update);
    public Task<ConversationData?>? GetAddProductConversation(long tgId);
    public Task IncrementStage(long id);
    public Task DecrementStage(long id);
    public Task AddCommandName(Update update);
    public Task AddProductId(Update update, long id);
    public Task DeleteConversation(ConversationData conversation);
}