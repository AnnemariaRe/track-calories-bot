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
    
    public EnterManuallyCommand(IUserRepo userRepo, IConversationDataRepo conversationRepo, IProductRepo productRepo)
    {
        _userRepo = userRepo;
        _conversationRepo = conversationRepo;
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
            var conversation = await _conversationRepo.GetAddProductConversation(message.Chat.Id)!;
            if (conversation is null) await _conversationRepo.CreateAddProductConversation(update);
            
            if (conversation?.CommandName is null)
            {
                await _conversationRepo.AddCommandName(update);
            }

            long? productId = 0;
            if (conversation.ProductId != null)
            {
                productId = conversation.ProductId;
            }

            switch (conversation.ConversationStage)
            {
                case 0:
                    await _conversationRepo.IncrementStage(message.Chat.Id);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write product name",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 1:
                    var product = await _productRepo.CreateProduct(update);
                    await _conversationRepo.AddProductId(update, product.Id);
                    await _conversationRepo.IncrementStage(message.Chat.Id);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Choose serving unit",
                        replyMarkup: InlineKeyboards.ServingTypeInlineKeyboard);
                    break;
                case 2:
                    await _productRepo.AddServingUnit(message.Text, productId);
                    await _conversationRepo.IncrementStage(message.Chat.Id);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write serving amount",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 3:
                    await _productRepo.AddServingAmount(message.Text, productId);
                    if (_productRepo.GetProduct(productId)!.Result.ServingAmount < 0)
                    {
                        await WrongAnswerMessage(message.Chat.Id, client);
                        break;
                    }
                    await _conversationRepo.IncrementStage(message.Chat.Id);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write calorie amount per 100 grams",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 4:
                    await _productRepo.AddCalorieAmount(message.Text, productId);
                    if (_productRepo.GetProduct(productId)!.Result.BaseCalories < 0)
                    {
                        await WrongAnswerMessage(message.Chat.Id, client);
                        break;
                    }
                    await _conversationRepo.IncrementStage(message.Chat.Id);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Do you want to add PFC info?",
                        replyMarkup: InlineKeyboards.YesOrNoInlineKeyboard);
                    break;
                case 5:
                    if (update.CallbackQuery?.Data is "yes")
                    {
                        await _conversationRepo.IncrementStage(message.Chat.Id);
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
                            await _conversationRepo.IncrementStage(message.Chat.Id);
                        }
                        goto case 9;
                    }
                    break;
                case 6:
                    await _productRepo.AddProtein(message.Text, productId);
                    if (_productRepo.GetProduct(productId)!.Result.BaseProtein < 0)
                    {
                        await WrongAnswerMessage(message.Chat.Id, client);
                        break;
                    }
                    await _conversationRepo.IncrementStage(message.Chat.Id);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write fat amount (per 100 grams)",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 7:
                    await _productRepo.AddFat(message.Text, productId);
                    if (_productRepo.GetProduct(productId)!.Result.BaseFat < 0)
                    {
                        await WrongAnswerMessage(message.Chat.Id, client);
                        break;
                    }
                    await _conversationRepo.IncrementStage(message.Chat.Id);
                    
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write carbs amount (per 100 grams)",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 8:
                    await _productRepo.AddCarbs(message.Text, productId);
                    if (_productRepo.GetProduct(productId)!.Result.BaseCarbs < 0)
                    {
                        await WrongAnswerMessage(message.Chat.Id, client);
                        break;
                    }
                    await _conversationRepo.IncrementStage(message.Chat.Id);

                    goto case 9;
                case 9:
                    await _conversationRepo.IncrementStage(message.Chat.Id);
                                        
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Write quantity (by default: 1)",
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
                case 10:
                    await _productRepo.AddQuantity(message.Text, productId);
                    if (_productRepo.GetProduct(productId)!.Result.Quantity < 0)
                    {
                        await WrongAnswerMessage(message.Chat.Id, client);
                        break;
                    }
                    await _conversationRepo.DeleteConversation(conversation);
                    
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