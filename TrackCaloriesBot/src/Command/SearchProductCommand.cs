using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Command;

public class SearchProductCommand : ICommand
{
    public SearchProductCommand(IUserRepo userRepo, IConversationDataRepo conversationRepo, IProductRepo productRepo, ISpoonacularRepo spoonacularRepo)
    {
        _userRepo = userRepo;
        _conversationRepo = conversationRepo;
        _productRepo = productRepo;
        _spoonacularRepo = spoonacularRepo;
    }

    public string Key => Commands.SearchProductsCommand;
    private readonly IUserRepo _userRepo;
    private readonly IConversationDataRepo _conversationRepo;
    private readonly IProductRepo _productRepo;
    private readonly ISpoonacularRepo _spoonacularRepo;
    
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
                message = update.CallbackQuery?.Message!;
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
            if (conversation is null) conversation = _conversationRepo.CreateConversation(update);
            
            if (conversation?.CommandName is null)
            {
                _conversationRepo.AddCommandName(text, message.Chat.Id);
            }
            
            long? productId = 0;
            if (conversation.ItemId != null) productId = conversation.ItemId;

            switch (conversation.ConversationStage)
            {
                case 0:
                    _conversationRepo.IncrementStage(message.Chat.Id);

                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Click on a button",
                        replyMarkup: InlineKeyboards.SearchInlineKeyboard);
                    break;
                case 1:
                    _conversationRepo.IncrementStage(message.Chat.Id);
                    goto case 2;
                case 2:
                    if (text is "/continue")
                    {
                        _conversationRepo.IncrementStage(message.Chat.Id);
                        goto case 3;
                    }
                    if (text is "/search")
                    {
                        var id1 = _conversationRepo.GetConversationData(message.Chat.Id)!.ItemId;
                        await _productRepo.DeleteProduct(id1);
                        _conversationRepo.AddItemId(message.Chat.Id, 0);
                        _conversationRepo.DecrementStage(message.Chat.Id);
                        goto case 0;
                    }
                    if (int.TryParse(message.Text, out var x))
                    {
                        var newProduct = await _productRepo.CreateProduct(update);
                        var productResult = _spoonacularRepo.GetProductInfo(x).Result;

                        _conversationRepo.AddItemId(message.Chat.Id, newProduct.Id);
                        await _productRepo.AddProductInfoFromResponse(productResult, newProduct.Id);
                        var product = await _productRepo.GetProduct(newProduct.Id)!;

                        var textMessage= $"<b>{product?.Name}</b> \n" + 
                                         "---------------------------------------\n" +
                                         $"<pre>Calories: {product?.BaseCalories} kcal \n" +
                                         $"Carbs:    {product?.BaseCarbs} g \n" +
                                         $"Protein:  {product?.BaseProtein} g \n" +
                                         $"Fat:      {product?.BaseFat} g </pre>\n";
        
                        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: textMessage,
                            parseMode: ParseMode.Html,
                            replyMarkup: InlineKeyboards.ProductInfoInlineKeyboard);
                    }
                    break;
                case 3:
                    _conversationRepo.IncrementStage(message.Chat.Id);

                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write serving amount (in grams)",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 4:
                    _conversationRepo.IncrementStage(message.Chat.Id);
                    var id2 = _conversationRepo.GetConversationData(message.Chat.Id)!.ItemId;
                    await _productRepo.AddServingAmount(text, id2);
                    
                    if (_productRepo.GetProduct(productId)!.Result!.ServingAmount < 0)
                    {
                        await WrongAnswerMessage(message.Chat.Id, client);
                        break;
                    }
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Enter quantity (by default: 1)",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 5:
                    var result = _conversationRepo.GetConversationData(message.Chat.Id)!;
                    await _productRepo.AddQuantity(text, result.ItemId);
                    
                    if (_productRepo.GetProduct(productId)!.Result!.Quantity < 0)
                    {
                        await WrongAnswerMessage(message.Chat.Id, client);
                        break;
                    }
                    _conversationRepo.DeleteConversation(result);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Successfully added!",
                        replyMarkup: KeyboardMarkups.MenuKeyboardMarkup);
                    break;
            }
        }
    }
    
    private async Task WrongAnswerMessage(long id, ITelegramBotClient client)
    {
        await client.SendTextMessageAsync(
            chatId: id,
            text: "Write a positive number, please",
            replyMarkup: new ReplyKeyboardRemove());
    }
}