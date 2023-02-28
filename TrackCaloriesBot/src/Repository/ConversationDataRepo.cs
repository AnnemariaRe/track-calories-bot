using Newtonsoft.Json;
using StackExchange.Redis;
using Telegram.Bot.Types;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Repository;

public class ConversationDataRepo : IConversationDataRepo
{
    private readonly IConnectionMultiplexer _redis;

    public ConversationDataRepo(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }
    
    public ConversationData? CreateAddProductConversation(Update update)
    {
        var db = _redis.GetDatabase();

        var conversation = db.StringGet(update.Message?.Chat.Id.ToString());
        if (!conversation.IsNullOrEmpty)
        {
            return JsonConvert.DeserializeObject<ConversationData>(conversation);
        }
        
        var newConversation = new ConversationData()
        {
            UserId = update.Message.Chat.Id,
            CommandName = null,
            MealType = update.Message.Text,
            ItemId = 0,
            ConversationStage = 0
        };
        
        var serialConversation = JsonConvert.SerializeObject(newConversation);
        db.StringSet(newConversation.UserId.ToString(), serialConversation);

        return newConversation;
    }

    public ConversationData? GetAddProductConversation(long tgId)
    {
        var db = _redis.GetDatabase();
        var conversation = db.StringGet(tgId.ToString());

        return !string.IsNullOrEmpty(conversation) ? JsonConvert.DeserializeObject<ConversationData>(conversation) : null;
    }

    public void IncrementStage(long id)
    {
        var db = _redis.GetDatabase();
        var conversation = GetAddProductConversation(id);
        if (conversation is not null)
        {
            conversation.ConversationStage++;
            var serialConversation = JsonConvert.SerializeObject(conversation);
            db.StringSet(id.ToString(), serialConversation);
        }
    }
    
    public void DecrementStage(long id)
    {
        var db = _redis.GetDatabase();
        var conversation = GetAddProductConversation(id);
        if (conversation is not null)
        {
            conversation.ConversationStage--;
            var serialConversation = JsonConvert.SerializeObject(conversation);
            db.StringSet(id.ToString(), serialConversation);
        }
    }
    
    public void AddProductId(Update update, long id)
    {
        if (update.Message?.Text == null) return;
        
        var db = _redis.GetDatabase();
        var conversation = GetAddProductConversation(update.Message.Chat.Id);
        if (conversation is not null)
        {
            conversation.ItemId = id;
            var serialConversation = JsonConvert.SerializeObject(conversation);
            db.StringSet(update.Message.Chat.Id.ToString(), serialConversation);
        }
    }

    public void AddCommandName(Update update)
    {
        if (update.Message?.Text == null) return;
        
        var db = _redis.GetDatabase();
        var conversation = GetAddProductConversation(update.Message.Chat.Id);
        if (conversation is not null)
        {
            conversation.CommandName = update.Message.Text;
            var serialConversation = JsonConvert.SerializeObject(conversation);
            db.StringSet(update.Message.Chat.Id.ToString(), serialConversation);
        }
    }

    public void DeleteConversation(ConversationData? conversation)
    {
        var db = _redis.GetDatabase();
        db.KeyDelete(conversation.UserId.ToString());
    }
}