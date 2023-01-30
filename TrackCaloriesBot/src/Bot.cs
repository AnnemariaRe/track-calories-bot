using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Microsoft.Extensions.Hosting;
using Telegram.Bot.Types.Enums;

namespace TrackCaloriesBot;

public class Bot
{
    public Bot(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private readonly IConfiguration _configuration;
    private TelegramBotClient _botClient;
    
    public async Task<TelegramBotClient> GetClient()
    {
        _botClient = new TelegramBotClient("");
        var hook = "";
        //await _botClient.SetWebhookAsync(hook, dropPendingUpdates: true);
        await _botClient.SetWebhookAsync(hook);

        return _botClient;
    }
}
