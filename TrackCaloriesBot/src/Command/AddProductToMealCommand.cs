using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Command;

public class AddProductToMealCommand : ICommand
{
    public string Key => Commands.AddProductToMealCommand;
    private readonly IUserRepo _userRepo;
    private readonly IConversationDataRepo _conversationRepo;
    private readonly IDayTotalDataRepo _dayTotalDataRepo;
    private readonly IMealDataRepo _mealDataRepo;
    
    public AddProductToMealCommand(IUserRepo userRepo, IConversationDataRepo conversationRepo, IDayTotalDataRepo dayTotalDataRepo, IMealDataRepo mealDataRepo)
    {
        _userRepo = userRepo;
        _conversationRepo = conversationRepo;
        _dayTotalDataRepo = dayTotalDataRepo;
        _mealDataRepo = mealDataRepo;
    }
    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        var message = update.Type is UpdateType.CallbackQuery ? update.CallbackQuery?.Message! : update.Message!;
        var userData = await _userRepo.GetUser(message.Chat.Id)!;
        
        if (userData is null)
        {
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Welcome to the Track Calories Bot!",
                replyMarkup: InlineKeyboards.StartInlineKeyboard);
        }
        else
        {
            if (await _dayTotalDataRepo.GetDayTotalData(update) is null)
                await _dayTotalDataRepo.AddNewDayTotalData(update);
            
            await _conversationRepo.CreateAddProductConversation(update);
            await _mealDataRepo.AddNewMealData(update);
            await _dayTotalDataRepo.AddNewMealType(update);

            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Choose how to add the product",
                replyMarkup: KeyboardMarkups.MealKeyboardMarkup);
        }
    }
}