using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TrackCaloriesBot.Command;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Repository.Interfaces;
using TrackCaloriesBot.Service.Interfaces;

namespace TrackCaloriesBot.Service;

public class CommandService : ICommandService
{
    private readonly List<ICommand> _commands;
    private readonly ICommandRepo _commandRepo;

    public CommandService(IServiceProvider serviceProvider, ICommandRepo commandRepo)
    {
        _commandRepo = commandRepo;
        _commands = serviceProvider.GetServices<ICommand>().ToList();
    }
    
    public async Task Execute(Update? update, TelegramBotClient client)
    {
        var id = update.Type switch
        {
            UpdateType.Message => update.Message?.Chat.Id,
            UpdateType.CallbackQuery => update.CallbackQuery?.Message?.Chat.Id,
            UpdateType.InlineQuery => update.InlineQuery?.From.Id,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        switch (update)
        {
            case { Type: UpdateType.InlineQuery, InlineQuery.Query.Length: > 3 and < 10 }:
                await ExecuteCommand(Commands.InlineCommand, id, update, client);
                return;
            case { Type: UpdateType.CallbackQuery }:
            {
                if (update.CallbackQuery?.Data != null && update.CallbackQuery.Data.Contains(Commands.RegisterCommand))
                {
                    await ExecuteCommand(Commands.RegisterCommand, id, update, client);
                }

                break;
            }
        }
        
        var messageText = update?.Message?.Text;
        var callbackQuery = update?.CallbackQuery;
        
        if (messageText is null && callbackQuery is null)
            return;
        
        switch (messageText)
        {
            case Commands.StartCommand:
                await ExecuteCommand(Commands.StartCommand, id, update, client);
                return;
            case Commands.ShowUserInfoCommand:
                await ExecuteCommand(Commands.ShowUserInfoCommand, id, update, client);
                return;
            case Commands.SummaryCommand:
                await ExecuteCommand(Commands.SummaryCommand, id, update, client);
                return;
            case Commands.NewRecordCommand:
                await ExecuteCommand(Commands.NewRecordCommand, id, update, client);
                return;
            case Commands.AddWaterCommand:
                await ExecuteCommand(Commands.AddWaterCommand, id, update, client);
                return;
            case Commands.BackCommand or Commands.DefaultBackCommand:
                await ExecuteCommand(Commands.BackCommand, id, update, client);
                return;
            case Commands.EnterManuallyCommand:
                await ExecuteCommand(Commands.EnterManuallyCommand, id, update, client);
                return;
            case Commands.SearchProductsCommand:
                await ExecuteCommand(Commands.SearchProductsCommand, id, update, client);
                return;
            case "Breakfast" or "Lunch" or "Dinner" or "Snack":
                await ExecuteCommand(Commands.AddProductToMealCommand, id, update, client);
                return;
        }

        var lastCommand = _commandRepo.GetLastCommand(id.ToString());
        switch (lastCommand?.CommandKey)
        {
            case Commands.RegisterCommand:
                await ExecuteCommand(Commands.RegisterCommand, id, update, client);
                break;
            case Commands.EnterManuallyCommand:
                if (messageText != Commands.EnterManuallyCommand || callbackQuery.Data != Commands.EnterManuallyCommand)
                {
                    await ExecuteCommand(Commands.EnterManuallyCommand, id, update, client);
                }
                break;
            case Commands.InlineCommand:
                await ExecuteCommand(Commands.SearchProductsCommand, id, update, client);
                break;
            case Commands.SearchProductsCommand:
                await ExecuteCommand(Commands.SearchProductsCommand, id, update, client);
                break;
        }
    }

    private async Task ExecuteCommand(string key, long? id, Update? update, ITelegramBotClient client)
    {
        _commandRepo.AddCommand(new LastCommand(id.ToString(), key));
        await _commands.First(x => x.Key == key).Execute(update,client);
    }

}