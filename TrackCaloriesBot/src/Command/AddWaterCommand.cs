using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Service;
using TrackCaloriesBot.Service.Interfaces;

namespace TrackCaloriesBot.Command;

public class AddWaterCommand : ICommand
{
    public string Key => Commands.AddWaterCommand;
    private readonly IUserService _userService;
    private readonly IDayTotalDataService _dayTotalDataService;
    
    public AddWaterCommand(IUserService userService, IDayTotalDataService dayTotalDataService)
    {
        _userService = userService;
        _dayTotalDataService = dayTotalDataService;
    }
    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        var message = update.Type is UpdateType.CallbackQuery ? update.CallbackQuery?.Message! : update.Message!;
        var userData = await _userService.GetUser(message.Chat.Id)!;
        
        if (userData is null)
        {
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Welcome to the Track Calories Bot!",
                replyMarkup: InlineKeyboards.StartInlineKeyboard);
            return;
        }
        
        if (message.Text == Key)
        {
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Write amount in L",
                replyMarkup: new ReplyKeyboardRemove());
        }
        else
        {
            if (await _dayTotalDataService.GetDayTotalData(update) is null)
            {
                await _dayTotalDataService.AddNewDayTotalData(update);
            }
            
            var check = float.TryParse(message.Text, out var x);
            if (check && x is > 0 and <= 5)
            {
                await _dayTotalDataService.AddWater(update, x);
                await client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Successfully added!",
                    replyMarkup: KeyboardMarkups.MenuKeyboardMarkup);
            }
            else
            {
                await client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Incorrect input, try again");
            }
        }
    }
}