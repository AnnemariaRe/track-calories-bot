using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Command;

public class AddWaterCommand : ICommand
{
    public string Key => Commands.AddWaterCommand;
    private readonly IUserRepo _userRepo;
    private readonly IDayTotalDataRepo _dayTotalDataRepo;
    
    public AddWaterCommand(IUserRepo userRepo, IDayTotalDataRepo dayTotalDataRepo)
    {
        _userRepo = userRepo;
        _dayTotalDataRepo = dayTotalDataRepo;
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
            if (await _dayTotalDataRepo.GetDayTotalData(update) is null)
            {
                await _dayTotalDataRepo.AddNewDayTotalData(update);
            }
            
            var check = float.TryParse(message.Text, out var x);
            if (check && x is > 0 and <= 5)
            {
                await _dayTotalDataRepo.AddWater(update, x);
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