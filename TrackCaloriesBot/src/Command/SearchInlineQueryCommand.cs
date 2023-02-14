using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using TrackCaloriesBot.Constant;
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
        if (update.Type == UpdateType.InlineQuery)
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
        }
    }
}