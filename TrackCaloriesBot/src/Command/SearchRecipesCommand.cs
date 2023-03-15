using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Exceptions;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Command;

public class SearchRecipesCommand : ICommand
{
    public string Key => Commands.SearchRecipesCommand;
    private readonly IUserRepo _userRepo;
    private readonly IConversationDataRepo _conversationRepo;
    private readonly ISpoonacularRepo _spoonacularRepo;
    private readonly IRequestRepo _requestRepo;
    private readonly IRecipeRepo _recipeRepo;

    public SearchRecipesCommand(IUserRepo userRepo, IConversationDataRepo conversationRepo,
        ISpoonacularRepo spoonacularRepo, IRequestRepo requestRepo, IRecipeRepo recipeRepo)
    {
        _userRepo = userRepo;
        _conversationRepo = conversationRepo;
        _spoonacularRepo = spoonacularRepo;
        _requestRepo = requestRepo;
        _recipeRepo = recipeRepo;
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
            var conversation = _conversationRepo.GetConversationData(message.Chat.Id)! ??
                               _conversationRepo.CreateConversation(update);

            if (conversation?.CommandName is null)
            {
                _conversationRepo.AddCommandName(text, message.Chat.Id);
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
                case 4:
                    if (int.TryParse(message.Text, out var x))
                    {
                        _conversationRepo.IncrementStage(message.Chat.Id);

                        var recipe = _spoonacularRepo.GetRecipeInfo(x).Result;
                        _conversationRepo.AddItemId(message.Chat.Id, x);

                        var textMessage = $"<b>{recipe?.Title}</b> \n \n" +
                                          $"<pre>Calories per serving: {recipe?.Nutrition.Nutrients.FirstOrDefault(i => i.Name == "Calories")?.Amount} \n" +
                                          $"Weight per serving: {recipe?.WeightPerServing} g \n" +
                                          $"Servings: {recipe?.Servings} \n" +
                                          $"Ready: {recipe?.ReadyInMinutes} minutes</pre>";

                        var webAppInlineKeyboard = new InlineKeyboardMarkup(new[]
                        {
                            new[] {
                                InlineKeyboardButton.WithWebApp("Full recipe info", new WebAppInfo() { Url = recipe?.SourceUrl })
                            },
                        });

                        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: textMessage,
                            parseMode: ParseMode.Html,
                            replyMarkup: webAppInlineKeyboard);
                        
                        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "What's next?",
                            replyMarkup: KeyboardMarkups.AfterRecipeInfoKeyboardMarkup);
                    }
                    break;
                case 5:
                    if (text == "Save to my favourites")
                    {
                        var recipe = _spoonacularRepo.GetRecipeInfo(_conversationRepo.GetConversationData(message.Chat.Id)!.ItemId).Result;
                        try
                        {
                            await _recipeRepo.CreateRecipeFromResponse(recipe, message.Chat.Id);
                        }
                        catch (BotException e)
                        {
                            await client.SendTextMessageAsync(
                                chatId: message.Chat.Id,
                                text: "Recipe already exists in your favourites");
                            return;
                        }
                        
                        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Recipe is successfully saved!",
                            replyMarkup: KeyboardMarkups.AfterRecipeInfoKeyboardMarkup);
                    } else if (text == "Get another recipe with same properties")
                    {
                        _conversationRepo.DecrementStage(message.Chat.Id);
                        _conversationRepo.DecrementStage(message.Chat.Id);
                        goto case 3;
                    } else if (text == "Get another recipe with new properties")
                    {
                        _conversationRepo.DeleteConversation(_conversationRepo.GetConversationData(message.Chat.Id)!);
                        _conversationRepo.CreateConversation(update);
                        goto case 0;
                    }
                    break;

            }
        }
    }
}