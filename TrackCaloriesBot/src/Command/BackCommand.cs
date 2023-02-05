using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Service;

namespace TrackCaloriesBot.Command;

public class BackCommand : ICommand
{
    public string Key => Commands.BackCommand;
    private readonly IUserService _userService;
    
    public BackCommand(IUserService userService)
    {
        _userService = userService;
    }
    
    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        var message = update.Type is UpdateType.CallbackQuery ? update.CallbackQuery?.Message! : update.Message!;
        var userData = await _userService.GetUser(message.Chat.Id)!;
        
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
                chatId: message.Chat.Id,
                text: "Choose action",
                replyMarkup: KeyboardMarkups.MenuKeyboardMarkup);
        }
    }
}