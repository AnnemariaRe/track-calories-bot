using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Entity.Requests;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Command;

public class SearchInlineQueryCommand : ICommand
{
    public SearchInlineQueryCommand(IConversationDataRepo conversationRepo, ISpoonacularRepo spoonacularRepo, IRequestRepo requestRepo)
    {
        _conversationRepo = conversationRepo;
        _spoonacularRepo = spoonacularRepo;
        _requestRepo = requestRepo;
    }

    public string Key => Commands.InlineCommand;
    private readonly IConversationDataRepo _conversationRepo;
    private readonly ISpoonacularRepo _spoonacularRepo;
    private readonly IRequestRepo _requestRepo;
    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        if (update.Type == UpdateType.InlineQuery)
        {
            var conversation = _conversationRepo.GetConversationData(update.InlineQuery.From.Id)!;
            
            if (conversation is { CommandName: Commands.SearchProductsCommand, ConversationStage: 1 })
            {
                var productsResult = _spoonacularRepo.GetProducts(update.InlineQuery.Query);
                var results = productsResult.Result
                    .Select(product =>
                        new InlineQueryResultArticle(product.Name.GetHashCode().ToString(), product.Name,
                            new InputTextMessageContent($"{product.Id}")) { ThumbUrl = product.Image })
                    .Cast<InlineQueryResult>().ToList();
                
                    await client.AnswerInlineQueryAsync(
                    update.InlineQuery.Id, results);
            }
            
            if (conversation is { CommandName: Commands.SearchRecipesCommand, ConversationStage: 4 })
            {
                var recipeResult = _spoonacularRepo.GetRecipes(_requestRepo.GetRequest(update.InlineQuery.From.Id), update.InlineQuery.Query);

                var results = recipeResult.Result.Select(recipe => new InlineQueryResultArticle(
                        recipe.Title.GetHashCode().ToString(), recipe.Title,
                        new InputTextMessageContent($"{recipe.Id}")) { ThumbUrl = recipe.Image })
                        .Cast<InlineQueryResult>().ToList();

                await client.AnswerInlineQueryAsync(
                    update.InlineQuery.Id, results);
            }
        }
    }
}