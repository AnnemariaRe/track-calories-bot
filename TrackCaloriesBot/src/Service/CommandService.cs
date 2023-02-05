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
    
    public CommandService(IServiceProvider serviceProvider)
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
            if (callbackQuery?.Data != null && callbackQuery.Data.Contains(Commands.RegisterCommand))
            {
                await ExecuteCommand(Commands.RegisterCommand, update, client);
            }
        }
        
        switch (messageText)
        {
            case Commands.StartCommand:
                await ExecuteCommand(Commands.StartCommand, update, client);
                break;
            case Commands.ShowUserInfoCommand:
                await ExecuteCommand(Commands.ShowUserInfoCommand, update, client);
                break;
            case Commands.SummaryCommand:
                await ExecuteCommand(Commands.SummaryCommand, update, client);
                break;
            case Commands.NewRecordCommand:
                await ExecuteCommand(Commands.NewRecordCommand, update, client);
                break;
            case Commands.AddWaterCommand:
                await ExecuteCommand(Commands.AddWaterCommand, update, client);
                break;
        }

        switch (_lastCommand?.Key)
        {
            case Commands.RegisterCommand:
                await ExecuteCommand(Commands.RegisterCommand, update, client);
                break;
            case Commands.NewRecordCommand:
                if (messageText is "Breakfast" or "Lunch" or "Dinner" or "Snack")
                {
                    await ExecuteCommand(Commands.NewRecordCommand, update, client);
                }
                break;
            case Commands.AddWaterCommand:
                if (messageText != Commands.AddWaterCommand)
                {
                    await ExecuteCommand(Commands.AddWaterCommand, update, client);
                }
                break;
            case null:
                break;
        }
    }

    private async Task ExecuteCommand(string key, Update? update, ITelegramBotClient client)
    {
        _lastCommand = _commands.First(x => x.Key == key);
        await _lastCommand.Execute(update, client);
    }

}