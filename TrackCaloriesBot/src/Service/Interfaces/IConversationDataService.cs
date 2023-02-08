using Telegram.Bot.Types;
using TrackCaloriesBot.Entity;

namespace TrackCaloriesBot.Service.Interfaces;

public interface IConversationDataService
{
    public Task<ConversationData> CreateAddProductConversation(Update update);
    public Task<ConversationData?>? GetAddProductConversation(long tgId);
    public Task IncrementStage(Update update);
    public Task AddCommandName(Update update);
    public Task AddProductId(Update update, long id);
    public Task DeleteAddProductConversation(ConversationData conversation);
}