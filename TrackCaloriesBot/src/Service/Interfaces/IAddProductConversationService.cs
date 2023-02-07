using Telegram.Bot.Types;
using TrackCaloriesBot.Entity;

namespace TrackCaloriesBot.Service.Interfaces;

public interface IAddProductConversationService
{
    public Task<AddProductConversation> CreateAddProductConversation(Update update);
    public Task<AddProductConversation?>? GetAddProductConversation(long tgId);
    public Task IncrementStage(Update update);
    public Task AddCommandName(Update update);
    public Task DeleteAddProductConversation(AddProductConversation conversation);
}