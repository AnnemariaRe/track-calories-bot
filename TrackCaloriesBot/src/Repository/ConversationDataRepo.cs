using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TrackCaloriesBot.Context;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Exceptions;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Repository;

public class ConversationDataRepo : IConversationDataRepo
{
    private readonly ApplicationDbContext _context;

    public ConversationDataRepo(ApplicationDbContext context) 
    {
        _context = context;
    }
    
    public async Task<ConversationData> CreateAddProductConversation(Update update)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x =>
            update.Message != null && x.TgId == update.Message.Chat.Id);

        var conversation =
            await _context.ProductConversations.FirstOrDefaultAsync(x => 
                update.Message != null && x.User.TgId == update.Message.Chat.Id);
        
        if (conversation != null) return conversation;
        
        var newConversation = new ConversationData()
        {
            Id = new Guid(),
            User = user,
            CommandName = null,
            MealType = update.Message.Text,
            ProductId = 0,
            ConversationStage = 0
        };

        var result = await _context.ProductConversations.AddAsync(newConversation);
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<ConversationData?>? GetAddProductConversation(long tgId)
    {
        var conversation = await _context.ProductConversations.FirstOrDefaultAsync(x => x.User.TgId == tgId);
        if (conversation is null)
        {
            throw new NullBotException("ConversationData entity is not found.");
        }
        return conversation;
    }

    public async Task IncrementStage(long id)
    {
        var conversation = GetAddProductConversation(id);
        if (conversation?.Result is not null)
        {
         conversation.Result.ConversationStage++;
         await _context.SaveChangesAsync();
        }
    }
    
    public async Task DecrementStage(long id)
    {
        var conversation = GetAddProductConversation(id);
        if (conversation?.Result != null && conversation.Result.ConversationStage != 0)
        {
            conversation.Result.ConversationStage--;
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task AddProductId(Update update, long id)
    {
        if (update.Message?.Text != null)
        {
            var conversation = GetAddProductConversation(update.Message.Chat.Id);
            if (conversation?.Result is not null)
            {
                conversation.Result.ProductId = id;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddCommandName(Update update)
    {
        if (update.Message?.Text != null)
        {
            var conversation = GetAddProductConversation(update.Message.Chat.Id);
            if (conversation?.Result is not null)
            {
                conversation.Result.CommandName = update.Message.Text;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task DeleteConversation(ConversationData? conversation)
    {
        _context.ProductConversations.Remove(conversation);
        await _context.SaveChangesAsync();
    }
}