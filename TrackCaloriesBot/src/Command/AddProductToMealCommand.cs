using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Service;
using TrackCaloriesBot.Service.Interfaces;

namespace TrackCaloriesBot.Command;

public class AddProductToMealCommand : ICommand
{
    public string Key => Commands.AddProductToMealCommand;
    private readonly IUserService _userService;
    private readonly IAddProductConversationService _conversationService;
    private readonly IDayTotalDataService _dayTotalDataService;
    private readonly IMealDataService _mealDataService;
    
    public AddProductToMealCommand(IUserService userService, IAddProductConversationService conversationService, IDayTotalDataService dayTotalDataService, IMealDataService mealDataService)
    {
        _userService = userService;
        _conversationService = conversationService;
        _dayTotalDataService = dayTotalDataService;
        _mealDataService = mealDataService;
    }
    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        var message = update.Type is UpdateType.CallbackQuery ? update.CallbackQuery?.Message! : update.Message!;
        var userData = await _userService.GetUser(message.Chat.Id)!;
        
        if (userData is null)
        {
            await client.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: "Welcome to the Track Calories Bot!",
                replyMarkup: InlineKeyboards.StartInlineKeyboard);
        }
        else
        {
            if (await _dayTotalDataService.GetDayTotalData(update) is null)
            {
                await _dayTotalDataService.AddNewDayTotalData(update);
            }
            try
            {
                await _conversationService.CreateAddProductConversation(update);
                await _mealDataService.AddNewMealData(update);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            
            await client.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: "Choose how to add the product",
                replyMarkup: KeyboardMarkups.MealKeyboardMarkup);
        }
    }
}