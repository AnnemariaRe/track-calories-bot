using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Command;

public class SearchInlineQueryCommand : ICommand
{
    public SearchInlineQueryCommand(IConversationDataRepo conversationRepo, ISpoonacularRepo spoonacularRepo)
    {
        _conversationRepo = conversationRepo;
        _spoonacularRepo = spoonacularRepo;
    }

    public string Key => Commands.InlineCommand;
    private readonly IConversationDataRepo _conversationRepo;
    private readonly ISpoonacularRepo _spoonacularRepo;
    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        if (update.Type == UpdateType.InlineQuery)
        {
            var conversation = _conversationRepo.GetConversationData(update.InlineQuery.From.Id)!;
            if (conversation?.ConversationStage is 1)
            {
                var productsResult = _spoonacularRepo.GetProducts(update.InlineQuery.Query);
                var results = productsResult.Result
                    .Select(product =>
                        new InlineQueryResultArticle(product.Title.GetHashCode().ToString(), product.Title,
                            new InputTextMessageContent($"{product.Id}")) { ThumbUrl = product.Image })
                    .Cast<InlineQueryResult>().ToList();

                if (results is null)
                {
                    await client.SendTextMessageAsync(
                        chatId: update.InlineQuery.Id,
                        text: "Product does not exist, try again",
                        replyMarkup: KeyboardMarkups.MenuKeyboardMarkup);
                }
                await client.AnswerInlineQueryAsync(
                    update.InlineQuery.Id, results);
            }
        }
    }
}