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
                    await _conversationService.IncrementStage(message.From.Id);
                    goto case 2;
                case 2:
                    if (text is "/continue")
                    {
                        await _conversationService.IncrementStage(message.From.Id);
                        goto case 3;
                    }
                    if (text is "/search")
                    {
                        await _conversationService.DecrementStage(message.From.Id);
                        goto case 0;
                    }
                    
                    var productResult = new ResponseProduct();
                    if (int.TryParse(message.Text, out var x))
                    {
                        productResult = _spoonacularService.GetProductInfo(x).Result;
                    }
                            
                    var textMessage= $"<b>{productResult.Title}</b> \n" + "---------------------------------------\n" +
                                     $"<pre>Calories: {productResult.Nutrition.Nutrients.FirstOrDefault(x => x.Name == "Calories").Amount} kcal \n" +
                                     $"Carbs:    {productResult.Nutrition.Nutrients.FirstOrDefault(x => x.Name == "Carbohydrates").Amount} g \n" +
                                     $"Protein:  {productResult.Nutrition.Nutrients.FirstOrDefault(x => x.Name == "Protein").Amount} g \n" +
                                     $"Fat:      {productResult.Nutrition.Nutrients.FirstOrDefault(x => x.Name == "Fat").Amount} g </pre>\n";
    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: textMessage,
                        parseMode: ParseMode.Html,
                        replyMarkup: InlineKeyboards.ProductInfoInlineKeyboard);

                    break;
                case 3:
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: ":)",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
            }

        }
    }
}