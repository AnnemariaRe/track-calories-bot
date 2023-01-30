using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TrackCaloriesBot.Command;
using TrackCaloriesBot.Constant;

namespace TrackCaloriesBot.Service;

public class CommandService : ICommandService
{
    private readonly List<ICommand> _commands;
    private ICommand? _lastCommand;
    
    public CommandService(IServiceProvider serviceProvider, IUserService userService)
    {
        _commands = serviceProvider.GetServices<ICommand>().ToList();
    }
    
    public async Task Execute(Update? update, TelegramBotClient client)
    {
        var messageText = update?.Message?.Text;
        var callbackQuery = update?.CallbackQuery;

        if (messageText is null && callbackQuery is null)
            return;

        if (update is { Type: UpdateType.CallbackQuery })
        {
            if (callbackQuery.Data.Contains("/register"))
            {
                await ExecuteCommand(Commands.RegisterCommand, update, client);
            }
            if (callbackQuery.Data.Contains("/show"))
            {
                await ExecuteCommand(Commands.ShowUserInfoCommand, update, client);
            }
            // if (callbackQuery.Data.Contains("/update"))
            // {
            //     await ExecuteCommand(Commands.RegisterCommand, update, client);
            // }
        }
        
        if (messageText != null && messageText.Contains(Commands.StartCommand))
        {
            await ExecuteCommand(Commands.StartCommand, update, client);
        }
        
        switch (_lastCommand?.Key)
        {
            case Commands.RegisterCommand:
            {
                await ExecuteCommand(Commands.RegisterCommand, update, client);
                break;
            }
            case null:
            {
                break;
            }
        }
    }

    private async Task ExecuteCommand(string key, Update? update, ITelegramBotClient client)
    {
        _lastCommand = _commands.First(x => x.Key == key);
        await _lastCommand.Execute(update, client);
    }

}