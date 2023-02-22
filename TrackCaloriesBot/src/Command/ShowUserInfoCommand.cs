using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Repository.Interfaces;
using User = TrackCaloriesBot.Entity.User;

namespace TrackCaloriesBot.Command;

public class ShowUserInfoCommand : ICommand
{
    private readonly IUserRepo _userRepo;
    public string Key => Commands.ShowUserInfoCommand;
    public ShowUserInfoCommand(IUserRepo userRepo)
        {
          _userRepo = userRepo;
        }
    
    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        var userData = await _userRepo.GetUser(update.Message.Chat.Id);

        if (userData != null)
        {
            await client.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: BeautifulOutput(userData),
                replyMarkup: KeyboardMarkups.MenuKeyboardMarkup,
                parseMode: ParseMode.Html);
        }
    }

    private string BeautifulOutput(User userData)
    {
        var names = new List<string>()
        {
            "Name", "Age", "Gender", "Goal", "Weight", "Height", 
            "Goal weight", "Activity level"
        };
        var data = new List<string>()
        {
            $"{userData.Name}", $"{userData.Age} years", $"{userData.Gender}", $"{userData.Goal} weight", 
            $"{userData.Weight} kg", $"{userData.Height} cm", $"{userData.GoalWeight} kg", 
            $"{userData.ActivityLevel}"
        };

        var maxNameLength = names.Select(name => name.Length).Max();
        var maxDataLength = data.Select(data => data.Length).Max();
        var result = "<pre>";

        for (var i = 0; i < names.Count; i++)
        {
            result += names[i].PadRight(maxNameLength + maxDataLength + 2 - data[i].Length);
            result += data[i] + "\n";
        }

        result += "</pre>";
        return result;
    }

}