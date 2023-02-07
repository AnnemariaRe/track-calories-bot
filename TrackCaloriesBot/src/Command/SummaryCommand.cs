using Telegram.Bot;
using Telegram.Bot.Types;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Enums;
using TrackCaloriesBot.Service;
using TrackCaloriesBot.Service.Interfaces;
using User = TrackCaloriesBot.Entity.User;

namespace TrackCaloriesBot.Command;

public class SummaryCommand : ICommand
{
    public string Key => Commands.SummaryCommand;
    private readonly IUserService _userService;
    
    public SummaryCommand(IUserService userService)
    {
        _userService = userService;
    }

    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        var userData = await _userService.GetUser(update.Message.Chat.Id)!;

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
                text: $"Recommended Intake: {HarrisBenedictFormula(userData):0.00} kcal per day");
        }
        
    }

    private double HarrisBenedictFormula(User userData)
    {
        double result = 0;
        if (userData.Gender is Gender.Female)
        {
            result = 447.593 + (9.247 * userData.Weight) + (3.098 * userData.Height) - (4.33 * userData.Age);
            switch (userData.ActivityLevel)
            {
                case ActivityLevel.Low:
                    result *= 1.2;
                    break;
                case ActivityLevel.Moderate:
                    result *= 1.375;
                    break;
                case ActivityLevel.High:
                    result *= 1.55;
                    break;
                case ActivityLevel.VeryHigh:
                    result *= 1.725;
                    break;
            }
        } 
        else
        {
            result = 88.362 + (13.397 * userData.Weight) + (4.799 * userData.Height) - (5.677 * userData.Age);
            switch (userData.ActivityLevel)
            {
                case ActivityLevel.Low:
                    result *= 1.2;
                    break;
                case ActivityLevel.Moderate:
                    result *= 1.375;
                    break;
                case ActivityLevel.High:
                    result *= 1.55;
                    break;
                case ActivityLevel.VeryHigh:
                    result *= 1.725;
                    break;
            }
        }

        return result;
    }
}