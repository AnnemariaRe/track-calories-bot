using Telegram.Bot;
using Telegram.Bot.Types;
using TrackCaloriesBot.Service;

namespace TrackCaloriesBot.Command;

public interface ICommand
{
    public string Key { get; }
    public Task Execute(Update? update, ITelegramBotClient client);
}