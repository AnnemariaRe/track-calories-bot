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
    
    public ConversationData? CreateConversation(Update update)
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
            RecipeId = 0,
            LastMessageId = 0,
            ConversationStage = 0
        };
        
        var serialConversation = JsonConvert.SerializeObject(newConversation);
        db.StringSet(newConversation.UserId.ToString(), serialConversation);

        return newConversation;
    }

    public ConversationData? GetConversationData(long tgId)
    {
        var db = _redis.GetDatabase();
        var conversation = db.StringGet(tgId.ToString());

        return !string.IsNullOrEmpty(conversation) ? JsonConvert.DeserializeObject<ConversationData>(conversation) : null;
    }

    public void IncrementStage(long id)
    {
        var db = _redis.GetDatabase();
        var conversation = GetConversationData(id);
        if (conversation is not null)
        {
            conversation.ConversationStage++;
            var serialConversation = JsonConvert.SerializeObject(conversation);
            db.StringSet(id.ToString(), serialConversation);
        }
    }
    
    public void IncrementStageBy(long id, int amount)
    {
        var db = _redis.GetDatabase();
        var conversation = GetConversationData(id);
        if (conversation is not null && amount > 0)
        {
            conversation.ConversationStage += amount;
            var serialConversation = JsonConvert.SerializeObject(conversation);
            db.StringSet(id.ToString(), serialConversation);
        }
    }
    
    public void DecrementStage(long id)
    {
        var db = _redis.GetDatabase();
        var conversation = GetConversationData(id);
        if (conversation is not null)
        {
            conversation.ConversationStage--;
            var serialConversation = JsonConvert.SerializeObject(conversation);
            db.StringSet(id.ToString(), serialConversation);
        }
    }
    
    public void DecrementStageBy(long id, int amount)
    {
        var db = _redis.GetDatabase();
        var conversation = GetConversationData(id);
        if (conversation is not null && amount > 0 && amount <= conversation.ConversationStage)
        {
            conversation.ConversationStage -= amount;
            var serialConversation = JsonConvert.SerializeObject(conversation);
            db.StringSet(id.ToString(), serialConversation);
        }
    }
    
    public void AddItemId(Update update, int id)
    {
        if (update.Message?.Text == null) return;
        
        var db = _redis.GetDatabase();
        var conversation = GetConversationData(update.Message.Chat.Id);
        if (conversation is not null)
        {
            conversation.ItemId = id;
            var serialConversation = JsonConvert.SerializeObject(conversation);
            db.StringSet(update.Message.Chat.Id.ToString(), serialConversation);
        }
    }
    
    public void AddRecipeId(Update update, int id)
    {
        if (update.Message == null) return;
        
        var db = _redis.GetDatabase();
        var conversation = GetConversationData(update.Message.Chat.Id);
        if (conversation is not null)
        {
            conversation.RecipeId = id;
            var serialConversation = JsonConvert.SerializeObject(conversation);
            db.StringSet(update.Message.Chat.Id.ToString(), serialConversation);
        }
    }

    public void AddLastMessageId(Update update, int id)
    {
        if (update.Message?.Text == null) return;
        
        var db = _redis.GetDatabase();
        var conversation = GetConversationData(update.Message.Chat.Id);
        if (conversation is not null)
        {
            conversation.LastMessageId = id;
            var serialConversation = JsonConvert.SerializeObject(conversation);
            db.StringSet(update.Message.Chat.Id.ToString(), serialConversation);
        }
    }

    public void AddCommandName(string? data, long id)
    {
        if (data == null) return;
        
        var db = _redis.GetDatabase();
        var conversation = GetConversationData(id);
        if (conversation is not null)
        {
            conversation.CommandName = data;
            var serialConversation = JsonConvert.SerializeObject(conversation);
            db.StringSet(id.ToString(), serialConversation);
        }
    }

    public void DeleteConversation(ConversationData? conversation)
    {
        var db = _redis.GetDatabase();
        db.KeyDelete(conversation.UserId.ToString());
    }
}