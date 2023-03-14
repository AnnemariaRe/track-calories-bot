using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Command;

public class BackCommand : ICommand
{
    public string Key => Commands.BackCommand;
    private readonly IUserRepo _userRepo;
    private readonly IConversationDataRepo _conversationRepo;
    private readonly IRecipeRepo _recipeRepo;
    private readonly IProductRepo _productRepo;
    
    public BackCommand(IUserRepo userRepo, IConversationDataRepo conversationRepo, IRecipeRepo recipeRepo, IProductRepo productRepo)
    {
        _userRepo = userRepo;
        _conversationRepo = conversationRepo;
        _recipeRepo = recipeRepo;
        _productRepo = productRepo;
    }
    
    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        var message = update.Type is UpdateType.CallbackQuery ? update.CallbackQuery?.Message! : update.Message!;
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
            if ((conversation.CommandName == Commands.AddIngredientCommand ||
                 conversation.CommandName == Commands.CreateRecipeCommand) && conversation.RecipeId != 0)
            {
                await _recipeRepo.DeleteRecipe(conversation.RecipeId);
            }
            
            if ((conversation.CommandName == Commands.EnterManuallyCommand ||
                conversation.CommandName == Commands.SearchProductsCommand) && conversation.ItemId != 0)
            {
                await _productRepo.DeleteProduct(conversation.ItemId);
            }
            _conversationRepo.DeleteConversation(conversation);

            await client.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Choose action",
            replyMarkup: KeyboardMarkups.MenuKeyboardMarkup);
        }
    }
}