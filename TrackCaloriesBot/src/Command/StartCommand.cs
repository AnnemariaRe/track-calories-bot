using Telegram.Bot;
using Telegram.Bot.Types;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Command;

public class StartCommand : ICommand
{
    public string Key => Commands.StartCommand;

    private readonly IUserRepo _userRepo;
    
    public StartCommand(IUserRepo userRepo)
    {
        _userRepo = userRepo;
    }
    
    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        var userData = await _userRepo.GetUser(update.Message.Chat.Id)!;
         
        if (userData is null)
        {
            await client.SendTextMessageAsync(
                 chatId: update.Message.Chat.Id,
                 text: "Welcome to the Track Calories Bot!",
                 replyMarkup: InlineKeyboards.StartInlineKeyboard);
        }
        else
        {
            await client.SendTextMessageAsync(
             chatId: update.Message.Chat.Id,
             text: $"Hello again, {userData.Name}!",
             replyMarkup: KeyboardMarkups.MenuKeyboardMarkup);
        }
    }
}