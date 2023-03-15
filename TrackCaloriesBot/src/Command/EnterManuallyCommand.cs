 using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Command;

public class EnterManuallyCommand : ICommand
{
    public string Key => Commands.EnterManuallyCommand;
    private readonly IUserRepo _userRepo;
    private readonly IConversationDataRepo _conversationRepo;
    private readonly IProductRepo _productRepo;
    private readonly IRecipeRepo _recipeRepo;
    
    public EnterManuallyCommand(IUserRepo userRepo, IConversationDataRepo conversationRepo, IProductRepo productRepo, IRecipeRepo recipeRepo)
    {
        _userRepo = userRepo;
        _conversationRepo = conversationRepo;
        _productRepo = productRepo;
        _recipeRepo = recipeRepo;
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
            if (conversation is null) _conversationRepo.CreateConversation(update);
            
            if (conversation?.CommandName is null || conversation?.CommandName is Commands.CreateRecipeCommand)
            {
                _conversationRepo.AddCommandName(Commands.AddIngredientCommand, message.Chat.Id);
            }

            var messageId = conversation!.LastMessageId;
            var sentMessage = new Message();
            long? productId = 0;
            if (conversation.ItemId != null)
            {
                productId = conversation.ItemId;
            }

            switch (conversation.ConversationStage)
            {
                case 0:
                    _conversationRepo.IncrementStageBy(message.Chat.Id, 3);
                    goto case 3;
                case 3:
                    _conversationRepo.IncrementStage(message.Chat.Id);
                    
                    if (_conversationRepo.GetConversationData(message.Chat.Id)?.CommandName is Commands.AddIngredientCommand)
                    {
                        await client.EditMessageTextAsync(
                            messageId: messageId,
                            chatId: message.Chat.Id,
                            text: "Write ingredient name");
                        break;
                    }
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write product name",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 4:
                    var product = await _productRepo.CreateProduct(update);
                    _conversationRepo.AddItemId(message.Chat.Id, product.Id);
                    _conversationRepo.IncrementStage(message.Chat.Id);
                    
                    sentMessage = await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Choose serving unit",
                        replyMarkup: InlineKeyboards.ServingTypeInlineKeyboard);
                    _conversationRepo.AddLastMessageId(message.Chat.Id, sentMessage.MessageId);
                    break;
                case 5:
                    await _productRepo.AddServingUnit(update.CallbackQuery?.Data, productId);
                    _conversationRepo.IncrementStage(message.Chat.Id);
                    
                    await client.EditMessageTextAsync(
                        messageId: messageId,
                        chatId: message.Chat.Id,
                        text: "Write serving amount");
                    break;
                case 6:
                    await _productRepo.AddServingAmount(message.Text, productId);
                    if (_productRepo.GetProduct(productId)!.Result.ServingAmount < 0)
                    {
                        await WrongAnswerMessage(message.Chat.Id, client);
                        break;
                    }
                    _conversationRepo.IncrementStage(message.Chat.Id);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write calorie amount per 100 grams",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 7:
                    await _productRepo.AddCalorieAmount(message.Text, productId);
                    if (_productRepo.GetProduct(productId)!.Result.BaseCalories < 0)
                    {
                        await WrongAnswerMessage(message.Chat.Id, client);
                        break;
                    }
                    _conversationRepo.IncrementStage(message.Chat.Id);
                    
                    sentMessage = await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Do you want to add PFC info?",
                        replyMarkup: InlineKeyboards.YesOrNoInlineKeyboard);
                    _conversationRepo.AddLastMessageId(message.Chat.Id, sentMessage.MessageId);
                    break;
                case 8:
                    if (update.CallbackQuery?.Data is "yes")
                    {
                        _conversationRepo.IncrementStage(message.Chat.Id);
                        await client.EditMessageTextAsync(
                            messageId: messageId,
                            chatId: message.Chat.Id,
                            text: "Write protein amount (per 100 grams)");
                        break;
                    }
                    if (update.CallbackQuery?.Data is "no")
                    {
                        for (var i = 0; i < 4; i++)
                        {
                            _conversationRepo.IncrementStage(message.Chat.Id);
                        }
                        goto case 12;
                    }
                    break;
                case 9:
                    await _productRepo.AddProtein(message.Text, productId);
                    if (_productRepo.GetProduct(productId)!.Result.BaseProtein < 0)
                    {
                        await WrongAnswerMessage(message.Chat.Id, client);
                        break;
                    }
                    _conversationRepo.IncrementStage(message.Chat.Id);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write fat amount (per 100 grams)",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 10:
                    await _productRepo.AddFat(message.Text, productId);
                    if (_productRepo.GetProduct(productId)!.Result.BaseFat < 0)
                    {
                        await WrongAnswerMessage(message.Chat.Id, client);
                        break;
                    }
                    _conversationRepo.IncrementStage(message.Chat.Id);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write carbs amount (per 100 grams)",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 11:
                    await _productRepo.AddCarbs(message.Text, productId);
                    if (_productRepo.GetProduct(productId)!.Result.BaseCarbs < 0)
                    {
                        await WrongAnswerMessage(message.Chat.Id, client);
                        break;
                    }
                    _conversationRepo.IncrementStage(message.Chat.Id);

                    goto case 12;
                case 12:
                    _conversationRepo.IncrementStage(message.Chat.Id);
                                        
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write quantity (by default: 1)",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 13:
                    await _productRepo.AddQuantity(message.Text, productId);
                    if (_productRepo.GetProduct(productId).Result.Quantity < 0)
                    {
                        await WrongAnswerMessage(message.Chat.Id, client);
                        break;
                    }
                    
                    if (conversation.CommandName is Commands.AddIngredientCommand)
                    {
                        var ingredient = _productRepo.GetProduct(productId).Result;
                        await _recipeRepo.AddProduct(ingredient, conversation.RecipeId);
                        _conversationRepo.AddCommandName(Commands.CreateRecipeCommand, message.Chat.Id);
                        _conversationRepo.DecrementStageBy(message.Chat.Id, 10);
                        
                        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Successfully added!");
                        sentMessage = await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Do you want to add one more?",
                            replyMarkup: InlineKeyboards.AddIngredientFinishInlineKeyboard);
                        
                        _conversationRepo.AddLastMessageId(message.Chat.Id, sentMessage.MessageId);
                    } else if (conversation.CommandName is Commands.EnterManuallyCommand)
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

    private async Task WrongAnswerMessage(long id, ITelegramBotClient client)
    {
        await client.SendTextMessageAsync(
            chatId: id,
            text: "Write a positive number, please",
            replyMarkup: new ReplyKeyboardRemove());
    }
}