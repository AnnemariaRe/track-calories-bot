using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Command;

public class ShowMyRecipesCommand : ICommand
{
    public ShowMyRecipesCommand(IUserRepo userRepo, IConversationDataRepo conversationRepo, IRecipeRepo recipeRepo)
    {
        _userRepo = userRepo;
        _conversationRepo = conversationRepo;
        _recipeRepo = recipeRepo;
    }

    public string Key => Commands.ShowMyRecipesCommand;
    private readonly IUserRepo _userRepo;
    private readonly IConversationDataRepo _conversationRepo;
    private readonly IRecipeRepo _recipeRepo;

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
            var conversation = _conversationRepo.GetConversationData(message.Chat.Id)! ??
                               _conversationRepo.CreateConversation(update);

            if (conversation?.CommandName is null) _conversationRepo.AddCommandName(text, message.Chat.Id);
            var messageId = conversation!.LastMessageId;

            if (text == Commands.ShowMyRecipesCommand)
            {
                if (GetRecipesInlineKeyboard(message.Chat.Id) != null)
                {
                    var sentMessage = await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Click for more information",
                        replyMarkup: GetRecipesInlineKeyboard(message.Chat.Id));
                    _conversationRepo.AddLastMessageId(message.Chat.Id, sentMessage.MessageId);
                }
            }
            else if (int.TryParse(text, out var x))
            {
                var recipe = _recipeRepo.GetRecipe(x).Result;
                if (recipe != null && messageId != 0)
                {
                    var recipeKeyboard = InlineKeyboards.RecipeInfoInlineKeyboard;
                    if (recipe.SourceUrl != null)
                    {
                        recipeKeyboard = new InlineKeyboardMarkup(new[]
                        {
                            new[] { InlineKeyboardButton.WithWebApp("Full recipe info", new WebAppInfo() { Url = recipe.SourceUrl }), },
                            new[] { InlineKeyboardButton.WithCallbackData("Delete recipe", "Delete recipe"), },
                            new[] { InlineKeyboardButton.WithCallbackData("Back to all", "Back to all") }
                        });
                    }
                    var sentMessage = await client.EditMessageTextAsync(chatId: message.Chat.Id,
                        messageId: messageId,
                        text: RecipeInfoOutput(recipe),
                        replyMarkup: recipeKeyboard,
                        parseMode: ParseMode.Html);
                    _conversationRepo.AddLastMessageId(message.Chat.Id, sentMessage.MessageId);
                    _conversationRepo.AddRecipeId(message.Chat.Id, x);
                }
            }
            else if (text == "Delete recipe")
            {
                await _recipeRepo.DeleteRecipe(conversation?.RecipeId);
                await client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Recipe is successfully deleted!",
                    replyMarkup: KeyboardMarkups.RecipesKeyboardMarkup);
            }
            else if (text == "Back to all")
            {
                if (GetRecipesInlineKeyboard(message.Chat.Id) != null && messageId != 0)
                {
                    var sentMessage = await client.EditMessageTextAsync(
                        chatId: message.Chat.Id,
                        text: "Click for more information",
                        messageId: messageId,
                        replyMarkup: GetRecipesInlineKeyboard(message.Chat.Id));
                    _conversationRepo.AddLastMessageId(message.Chat.Id, sentMessage.MessageId);
                }
            }
        }
    }
    
    private string RecipeInfoOutput(Recipe recipe)
    {
        var output = $"<pre>Title: {recipe.Name} \n";
        double totalCalories = 0;
        double totalWeight = 0;
        
        output += "Ingredients: \n";
        foreach (var product in recipe.Products)
        {
            output += $"- {product.Quantity} x {product.ServingAmount} {product.ServingType} {product.Name} \n";
            totalCalories += product.BaseCalories * product.Quantity;
            totalWeight += product.ServingAmount * product.Quantity;
        }
        
        if (recipe.ApiId is null)
        {
            output += $"Servings: {recipe.ServingsNumber}\n" +
                      $"Calories per serving: {(totalCalories / totalWeight * (totalWeight / recipe.ServingsNumber)):0.00}\n" +
                      $"Weight per serving: {(totalWeight / recipe.ServingsNumber):0} g</pre>";
            if (recipe.Description != null)
            {
                output += $"<pre>\n\nDescription: \n{recipe.Description}</pre>";
            }

            return output;
        }

        output += $"Servings: {recipe.ServingsNumber}\n" +
                  $"Calories per serving: {recipe.BaseCalories:0.00}\n" +
                  $"Weight per serving: {recipe.WeightPerServing:0} g</pre>";
        
        return output;
    }

    private InlineKeyboardMarkup? GetRecipesInlineKeyboard(long id)
    {
        var recipes = _recipeRepo.GetAllRecipes(id).Result;
        if (recipes != null)
        {
            var showRecipesInlineKeyboard = new InlineKeyboardMarkup(
                recipes.Select(recipe => new[]
                {
                    InlineKeyboardButton.WithCallbackData(recipe.Name!, recipe.Id.ToString())
                }));
            return showRecipesInlineKeyboard;
        }

        return null;
    }
}