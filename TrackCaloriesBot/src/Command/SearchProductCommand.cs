using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Service.Interfaces;

namespace TrackCaloriesBot.Command;

public class SearchProductCommand : ICommand
{
    public SearchProductCommand(IUserService userService, IConversationDataService conversationService, IProductService productService, ISpoonacularService spoonacularService)
    {
        _userService = userService;
        _conversationService = conversationService;
        _productService = productService;
        _spoonacularService = spoonacularService;
    }

    public string Key => Commands.SearchProductsCommand;
    private readonly IUserService _userService;
    private readonly IConversationDataService _conversationService;
    private readonly IProductService _productService;
    private readonly ISpoonacularService _spoonacularService;
    
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
        
        var userData = await _userService.GetUser(message.Chat.Id)!;
        
        if (userData is null)
        {
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Welcome to the Track Calories Bot!",
                replyMarkup: InlineKeyboards.StartInlineKeyboard);
        }
        else
        {
            var conversation = await _conversationService.GetAddProductConversation(message.Chat.Id)!;
            if (conversation is null) await _conversationService.CreateAddProductConversation(update);
            
            if (conversation?.CommandName is null)
            {
                await _conversationService.AddCommandName(update);
            }
            
            long? productId = 0;
            if (conversation.ProductId != null) productId = conversation.ProductId;

            switch (conversation.ConversationStage)
            {
                case 0:
                    await _conversationService.IncrementStage(message.Chat.Id);

                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Click on a button",
                        replyMarkup: InlineKeyboards.SearchInlineKeyboard);
                    break;
                case 1:
                    await _conversationService.IncrementStage(message.Chat.Id);
                    goto case 2;
                case 2:
                    if (text is "/continue")
                    {
                        await _conversationService.IncrementStage(message.Chat.Id);
                        goto case 3;
                    }
                    if (text is "/search")
                    {
                        var id1 = _conversationService.GetAddProductConversation(message.Chat.Id)!.Result!.ProductId;
                        await _productService.DeleteProduct(id1);
                        await _conversationService.DecrementStage(message.Chat.Id);
                        goto case 0;
                    }
                    if (int.TryParse(message.Text, out var x))
                    {
                        var productResult = _spoonacularService.GetProductInfo(x).Result;

                        
                        var newProduct = await _productService.CreateProduct(update);
                        await _conversationService.AddProductId(update, newProduct.ProductId);
                        await _productService.AddProductInfoFromResponse(productResult, newProduct.ProductId);
                        var product = await _productService.GetProduct(newProduct.ProductId)!;

                        
                                
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
                    await _conversationService.IncrementStage(message.Chat.Id);

                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write serving amount (in grams)",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 4:
                    await _conversationService.IncrementStage(message.Chat.Id);
                    var id2 = _conversationService.GetAddProductConversation(message.Chat.Id)!.Result!.ProductId;
                    await _productService.AddServingAmount(text, id2);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Enter quantity (by default: 1)",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 5:
                    var result = _conversationService.GetAddProductConversation(message.Chat.Id)!.Result;
                    await _productService.AddQuantity(text, result.ProductId);
                    await _conversationService.DeleteConversation(result);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Successfully added!",
                        replyMarkup: KeyboardMarkups.MenuKeyboardMarkup);
                    break;
            }

        }
    }
}