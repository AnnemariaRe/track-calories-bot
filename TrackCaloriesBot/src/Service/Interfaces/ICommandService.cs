using Telegram.Bot;
using Telegram.Bot.Types;

namespace TrackCaloriesBot.Service.Interfaces;

public interface ICommandService
{
    public Task Execute(Update? update, TelegramBotClient client);
}