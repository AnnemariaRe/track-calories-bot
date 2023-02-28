using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Command;

public class SearchRecipesCommand : ICommand
{
    public string Key => Commands.SearchRecipesCommand;
    private readonly IUserRepo _userRepo;
    private readonly IConversationDataRepo _conversationRepo;
    private readonly ISpoonacularRepo _spoonacularRepo;
    private readonly IRequestRepo _requestRepo;

    public SearchRecipesCommand(IUserRepo userRepo, IConversationDataRepo conversationRepo, ISpoonacularRepo spoonacularRepo, IRequestRepo requestRepo)
    {
        _userRepo = userRepo;
        _conversationRepo = conversationRepo;
        _spoonacularRepo = spoonacularRepo;
        _requestRepo = requestRepo;
    }

    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        Message message;
        string? text;
        switch (update.Type)
        {
            case UpdateType.Message:
                message = update.Message!;
                text = update.Message?.Text;
                break;
            case UpdateType.CallbackQuery:
                message = update.CallbackQuery?.Message;
                text = update.CallbackQuery?.Data;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        } 
        
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
            var conversation = _conversationRepo.GetConversationData(message.Chat.Id)!;
            if (conversation is null) _conversationRepo.CreateConversation(update);
            
            if (conversation?.CommandName is null)
            {
                _conversationRepo.AddCommandName(update);
            }
            
            long? recipeId = 0;
            if (conversation?.ItemId != 0) recipeId = conversation?.ItemId;

            switch (conversation?.ConversationStage)
            {
                case 0:
                    _conversationRepo.IncrementStage(message.Chat.Id);
                    _requestRepo.CreateRequest(update);

                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write required ingredients \n comma separated",
                        replyMarkup: InlineKeyboards.SkipInlineKeyboard);
                    break;
                case 1: 
                    _conversationRepo.IncrementStage(message.Chat.Id);
                    if (text != "/skip")
                    {
                        _requestRepo.AddIngredients(message.Chat.Id, text);
                    }
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write the equipment required",
                        replyMarkup: InlineKeyboards.SkipInlineKeyboard);
                    break;
                case 2:
                    _conversationRepo.IncrementStage(message.Chat.Id);
                    if (text != "/skip")
                    {
                        _requestRepo.AddEquipments(message.Chat.Id, text);
                    }
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write max ready time in minutes",
                        replyMarkup: InlineKeyboards.SkipInlineKeyboard);
                    break;
                case 3:
                    _conversationRepo.IncrementStage(message.Chat.Id);
                    if (text != "/skip")
                    {
                        _requestRepo.AddMaxReadyTime(message.Chat.Id, text);
                    }
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Search with a recipe name",
                        replyMarkup: InlineKeyboards.SearchRecipeInlineKeyboard);
                    break;
            }
        }
    }
}