using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Command;

public class CreateRecipeCommand : ICommand
{
    public CreateRecipeCommand(IUserRepo userRepo, IConversationDataRepo conversationRepo, IRecipeRepo recipeRepo)
    {
        _userRepo = userRepo;
        _conversationRepo = conversationRepo;
        _recipeRepo = recipeRepo;
    }

    public string Key => Commands.CreateRecipeCommand;
    
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
            var conversation = _conversationRepo.GetConversationData(message.Chat.Id)!;
            if (conversation is null) conversation = _conversationRepo.CreateConversation(update);
            
            if (conversation?.CommandName is null) _conversationRepo.AddCommandName(text, message.Chat.Id);

            var recipeId = 0;
            if (conversation?.RecipeId != null) recipeId = conversation.RecipeId;

            switch (conversation.ConversationStage)
            {
                case 0:
                    _conversationRepo.IncrementStage(message.Chat.Id);

                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write recipe name",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 1:
                    var recipe = await _recipeRepo.CreateRecipe(update);
                    _conversationRepo.AddRecipeId(message.Chat.Id, recipe.Id); 
                    _conversationRepo.IncrementStage(message.Chat.Id);

                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "How many servings it has?",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 2:
                    await _recipeRepo.AddServingsNumber(text, recipeId);
                    _conversationRepo.IncrementStage(message.Chat.Id);

                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write included ingredients",
                        replyMarkup: InlineKeyboards.AddIngredientInlineKeyboard);
                    break;
                case 3:
                    if (text is "/finish")
                    {
                        _conversationRepo.IncrementStage(message.Chat.Id);
                        goto case 4;
                    }
                    break;
                case 4:
                    _conversationRepo.IncrementStage(message.Chat.Id);
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "How long it takes to prepare? (in minutes)");
                    break;
                case 5:
                    await _recipeRepo.AddReadyInMinutes(text, recipeId);
                    _conversationRepo.IncrementStage(message.Chat.Id);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Do you want to add a recipe description?",
                        replyMarkup: InlineKeyboards.YesOrNoInlineKeyboard);
                    break;
                case 6:
                    if (text is "yes")
                    {
                        _conversationRepo.IncrementStage(message.Chat.Id);
                        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Write a description");
                    } else if (text is "no")
                    {
                        _conversationRepo.IncrementStageBy(message.Chat.Id, 2);
                        goto case 8;
                    }
                    break;
                case 7:
                    await _recipeRepo.AddDescription(text, recipeId);
                    _conversationRepo.IncrementStage(message.Chat.Id);
                    goto case 8;
                case 8:
                    var recipeInfo = _recipeRepo.GetRecipe(recipeId).Result;
                    await _recipeRepo.AddAllCalories(recipeId);
                    await _recipeRepo.AddPFC(recipeId);
                    await _recipeRepo.AddWeightPerServing(recipeId);
                    _conversationRepo.IncrementStage(message.Chat.Id);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Your recipe info");
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: RecipeInfoOutput(recipeInfo!),
                        replyMarkup: KeyboardMarkups.GreatKeyboardMarkup,
                        parseMode: ParseMode.Html);
                    if (recipeInfo?.Description != null)
                    {
                        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: $"<pre>Recipe description: \n{recipeInfo.Description}</pre>",
                            parseMode: ParseMode.Html);
                    }
                    break;
                case 9:
                    if (text is "Great!")
                    {
                        _conversationRepo.DeleteConversation(conversation);
                        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Successfully added!",
                            replyMarkup: KeyboardMarkups.MenuKeyboardMarkup);
                    }
                    break;
            }
        }
    }

    private string RecipeInfoOutput(Recipe recipe)
    {
        var output = $"<pre>Title: {recipe.Name} \n" + "Ingredients: \n";
        double totalCalories = 0;
        double totalWeight = 0;
        foreach (var product in recipe.Products!)
        {
            output += $"- {product.Quantity} x {product.ServingAmount} {product.ServingType} {product.Name} \n";
            totalCalories += product.BaseCalories * product.Quantity;
            totalWeight += product.ServingAmount * product.Quantity;
        }

        output += $"Servings: {recipe.ServingsNumber}\n" +
                  $"Calories per serving: {(totalCalories / totalWeight * (totalWeight / recipe.ServingsNumber)):0.00}\n" +
                  $"Weight per serving: {(totalWeight / recipe.ServingsNumber):0} g</pre>";
        return output;
    }
}