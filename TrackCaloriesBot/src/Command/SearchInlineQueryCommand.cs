using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Service.Interfaces;

namespace TrackCaloriesBot.Command;

public class SearchInlineQueryCommand : ICommand
{
    public SearchInlineQueryCommand(IConversationDataService conversationService, ISpoonacularService spoonacularService)
    {
        _conversationService = conversationService;
        _spoonacularService = spoonacularService;
    }

    public string Key => Commands.InlineCommand;
    private readonly IConversationDataService _conversationService;
    private readonly ISpoonacularService _spoonacularService;
    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        switch (update.Type)
        {
            case UpdateType.InlineQuery:
            {
                var conversation = await _conversationService.GetAddProductConversation(update.InlineQuery.From.Id)!;
                if (conversation?.ConversationStage is 1)
                {
                    var productsResult = _spoonacularService.GetProducts(update.InlineQuery.Query);
                    var results = productsResult.Result
                        .Select(product =>
                            new InlineQueryResultArticle(product.Title.GetHashCode().ToString(), product.Title,
                                new InputTextMessageContent($"{product.ProductId}")) { ThumbUrl = product.Image })
                        .Cast<InlineQueryResult>().ToList();
                
                    await client.AnswerInlineQueryAsync(
                        update.InlineQuery.Id, results);
                }

                break;
            }
            case UpdateType.Message:
            {
                var conversation = await _conversationService.GetAddProductConversation(update.Message.From.Id)!;
                switch (conversation?.ConversationStage)
                {
                    case 1:
                        await _conversationService.IncrementStage(update.Message.From.Id);
                        goto case 2;
                    case 2:
                        ResponseProduct? productResult = null;
                        if (int.TryParse(update.Message.Text, out var x))
                        {
                            productResult = _spoonacularService.GetProductInfo(x).Result;
                        }
                        
                        var message = $"<b>{productResult.Title}</b> \n" + "---------------------\n" +
                                      $"<pre>Calories: {productResult.Nutrition.Nutrients.FirstOrDefault(x => x.Name == "Calories").Amount} kcal \n" +
                                      $"Carbs:    {productResult.Nutrition.Nutrients.FirstOrDefault(x => x.Name == "Carbohydrates").Amount} g \n" +
                                      $"Protein:  {productResult.Nutrition.Nutrients.FirstOrDefault(x => x.Name == "Protein").Amount} g \n" +
                                      $"Fat:     {productResult.Nutrition.Nutrients.FirstOrDefault(x => x.Name == "Fat").Amount} g </pre>\n";

                        await client.SendTextMessageAsync(
                            chatId: update.Message.From.Id,
                            text: message,
                            parseMode: ParseMode.Html,
                            replyMarkup: InlineKeyboards.ProductInfoInlineKeyboard);
                        break;
                }

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}