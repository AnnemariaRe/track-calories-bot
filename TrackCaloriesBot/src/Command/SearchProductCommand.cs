using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Service.Interfaces;

namespace TrackCaloriesBot.Command;

public class SearchProductCommand : ICommand
{
    public SearchProductCommand(IUserService userService, IConversationDataService conversationService, IProductService productService)
    {
        _userService = userService;
        _conversationService = conversationService;
        _productService = productService;
    }

    public string Key => Commands.SearchProductsCommand;
    private readonly IUserService _userService;
    private readonly IConversationDataService _conversationService;
    private readonly IProductService _productService;
    private readonly ISpoonacularService _spoonacularService;
    
    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        Message message;
        switch (update.Type)
        {
            case UpdateType.Message:
                message = update.Message!;
                break;
            case UpdateType.CallbackQuery:
                message = update.CallbackQuery?.Message!;
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
                    // var productsResult = _spoonacularService.GetProducts(message.Text);
                    //
                    // await client.AnswerInlineQueryAsync();
                    break;
            }

        }
    }
}