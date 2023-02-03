using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Service;

namespace TrackCaloriesBot.Command;

public class NewRecordCommand : ICommand
{
    public string Key => Commands.NewRecordCommand;
    private readonly IUserService _userService;
    
    public NewRecordCommand(IUserService userService)
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
                chatId: update.Message.Chat.Id,
                text: "Choose record type",
                replyMarkup: KeyboardMarkups.NewRecordKeyboardMarkup);
        }
    }
}