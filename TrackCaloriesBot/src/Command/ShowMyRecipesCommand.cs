using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TrackCaloriesBot.Constant;
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

            var recipes = _recipeRepo.GetAllRecipes(message.Chat.Id).Result;
            if (recipes != null)
            {
                var showRecipesInlineKeyboard = new InlineKeyboardMarkup(
                    recipes.Select(recipe => new[]
                    {
                        InlineKeyboardButton.WithCallbackData(recipe.Name!, recipe.Id.ToString())
                    }));
                
                await client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Click for more information",
                    replyMarkup: showRecipesInlineKeyboard);
            }
        }
    }
}