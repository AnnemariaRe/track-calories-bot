using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Service.Interfaces;

namespace TrackCaloriesBot.Command;

public class EnterManuallyCommand : ICommand
{
    public string Key => Commands.EnterManuallyCommand;
    private readonly IUserService _userService;
    private readonly IConversationDataService _conversationService;
    private readonly IProductService _productService;
    
    public EnterManuallyCommand(IUserService userService, IConversationDataService conversationService, IProductService productService)
    {
        _userService = userService;
        _conversationService = conversationService;
        _productService = productService;
    }
    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        var message = update.Type is UpdateType.CallbackQuery ? update.CallbackQuery?.Message! : update.Message!;
        var userData = await _userService.GetUser(message.Chat.Id)!;
        
        if (userData is null)
        {
            await client.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: "Welcome to the Track Calories Bot!",
                replyMarkup: InlineKeyboards.StartInlineKeyboard);
        }
        else
        {
            var conversation = await _conversationService.GetAddProductConversation(message.Chat.Id)!;
            if (conversation is null) await _conversationService.CreateAddProductConversation(update);
            
            if (conversation?.CommandName is null)
            {
                await _conversationService.AddCommandName(update);
            }

            long? productId = 0;
            if (conversation.ProductId != null)
            {
                productId = conversation.ProductId;
            }

            switch (conversation.ConversationStage)
            {
                case 0:
                    await _conversationService.IncrementStage(update);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write product name",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 1:
                    var product = await _productService.CreateProduct(update);
                    await _conversationService.AddProductId(update, product.ProductId);
                    await _conversationService.IncrementStage(update);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Choose serving unit",
                        replyMarkup: InlineKeyboards.ServingTypeInlineKeyboard);
                    break;
                case 2:
                    await _productService.AddServingUnit(update, productId);
                    await _conversationService.IncrementStage(update);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write serving amount",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 3:
                    await _productService.AddServingAmount(update, productId);
                    if (_productService.GetProduct(productId)!.Result.ServingAmount < 0)
                    {
                        await WrongAnswerMessage(message.Chat.Id, client);
                        break;
                    }
                    await _conversationService.IncrementStage(update);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write calorie amount per 100 grams",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 4:
                    await _productService.AddCalorieAmount(update, productId);
                    if (_productService.GetProduct(productId)!.Result.BaseCalories < 0)
                    {
                        await WrongAnswerMessage(message.Chat.Id, client);
                        break;
                    }
                    await _conversationService.IncrementStage(update);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Do you want to add PFC info?",
                        replyMarkup: InlineKeyboards.YesOrNoInlineKeyboard);
                    break;
                case 5:
                    if (update.CallbackQuery?.Data is "yes")
                    {
                        await _conversationService.IncrementStage(update);
                        await client.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Write protein amount (per 100 grams)",
                            replyMarkup: new ReplyKeyboardRemove());
                        break;
                    }
                    if (update.CallbackQuery?.Data is "no")
                    {
                        for (var i = 0; i < 4; i++)
                        {
                            await _conversationService.IncrementStage(update);
                        }
                        goto case 9;
                    }
                    break;
                case 6:
                    await _productService.AddProtein(update, productId);
                    if (_productService.GetProduct(productId)!.Result.BaseProtein < 0)
                    {
                        await WrongAnswerMessage(message.Chat.Id, client);
                        break;
                    }
                    await _conversationService.IncrementStage(update);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write fat amount (per 100 grams)",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 7:
                    await _productService.AddFat(update, productId);
                    if (_productService.GetProduct(productId)!.Result.BaseFat < 0)
                    {
                        await WrongAnswerMessage(message.Chat.Id, client);
                        break;
                    }
                    await _conversationService.IncrementStage(update);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write carbs amount (per 100 grams)",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 8:
                    await _productService.AddCarbs(update, productId);
                    if (_productService.GetProduct(productId)!.Result.BaseCarbs < 0)
                    {
                        await WrongAnswerMessage(message.Chat.Id, client);
                        break;
                    }
                    await _conversationService.IncrementStage(update);

                    goto case 9;
                case 9:
                    await _conversationService.IncrementStage(update);
                                        
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write quantity (by default: 1)",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 10:
                    await _productService.AddQuantity(update, productId);
                    if (_productService.GetProduct(productId)!.Result.Quantity < 0)
                    {
                        await WrongAnswerMessage(message.Chat.Id, client);
                        break;
                    }
                    await _conversationService.DeleteAddProductConversation(conversation);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Successfully added!",
                        replyMarkup: KeyboardMarkups.MenuKeyboardMarkup);
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