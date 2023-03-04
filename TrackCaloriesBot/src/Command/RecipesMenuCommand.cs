using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Command;

public class RecipesMenuCommand : ICommand
{
    public string Key => Commands.RecipesCommand;
    private readonly IUserRepo _userRepo;
    private readonly IConversationDataRepo _conversationRepo;
    
    public RecipesMenuCommand(IUserRepo userRepo, IConversationDataRepo conversationRepo)
    {
        _userRepo = userRepo;
        _conversationRepo = conversationRepo;
    }
    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        var message = update.Type is UpdateType.CallbackQuery ? update.CallbackQuery?.Message! : update.Message!;
        
        var userData = await _userRepo.GetUser(message.Chat.Id)!;
        
        if (userData is null)
        {
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Welcome to the Track Calories Bot!",
                replyMarkup: InlineKeyboards.StartInlineKeyboard);
        }
        else
        {
            _conversationRepo.CreateConversation(update);
            
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Choose your action",
                replyMarkup: KeyboardMarkups.RecipesKeyboardMarkup);
        }
    }
}