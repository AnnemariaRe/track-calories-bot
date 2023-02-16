using Microsoft.Extensions.Configuration;
using Telegram.Bot;

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
        _botClient = new TelegramBotClient(_configuration["Token"]);
        var hook = _configuration["Url"];
        //await _botClient.SetWebhookAsync(hook, dropPendingUpdates: true);
        await _botClient.SetWebhookAsync(hook);

        return _botClient;
    }
}
