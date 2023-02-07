using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TrackCaloriesBot.Context;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Service.Interfaces;

namespace TrackCaloriesBot.Service;

public class AddProductConversationService : IAddProductConversationService
{
    private readonly ApplicationDbContext _context;

    public AddProductConversationService(ApplicationDbContext context) 
    {
        _context = context;
    }
    
    public async Task<AddProductConversation> CreateAddProductConversation(Update update)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x =>
            update.Message != null && x.TgId == update.Message.Chat.Id);

        var conversation =
            await _context.ProductConversations.FirstOrDefaultAsync(x => 
                update.Message != null && x.User.TgId == update.Message.Chat.Id);


        if (conversation != null) return conversation;
        
        var newConversation = new AddProductConversation()
        {
            Id = new Guid(),
            User = user,
            CommandName = null,
            MealType = update.Message.Text,
            ConversationStage = 0
        };

        var result = await _context.ProductConversations.AddAsync(newConversation);
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<AddProductConversation?>? GetAddProductConversation(long tgId)
    {
        var conversation = await _context.ProductConversations.FirstOrDefaultAsync(x => x.User.TgId == tgId);
        return conversation;
    }

    public async Task IncrementStage(Update update)
    {
        if (update.Message?.Text != null)
        {
            var user = GetAddProductConversation(update.Message.Chat.Id);
            if (user?.Result is not null)
            {
                user.Result.ConversationStage++;
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

    public async Task DeleteAddProductConversation(AddProductConversation? conversation)
    {
        _context.ProductConversations.Remove(conversation);
        await _context.SaveChangesAsync();
    }
}