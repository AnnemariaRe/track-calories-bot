using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Service;
using TrackCaloriesBot.Service.Interfaces;

namespace TrackCaloriesBot.Command;

public class BackCommand : ICommand
{
    public string Key => Commands.BackCommand;
    private readonly IUserService _userService;
    private readonly IAddProductConversationService _conversationService;
    
    public BackCommand(IUserService userService, IAddProductConversationService conversationService)
    {
        _userService = userService;
        _conversationService = conversationService;
    }
    
    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        var message = update.Type is UpdateType.CallbackQuery ? update.CallbackQuery?.Message! : update.Message!;
        var userData = await _userService.GetUser(message.Chat.Id)!;
        
        if (userData is null)
        {
            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Welcome to the Track Calories Bot!",
                replyMarkup: InlineKeyboards.StartInlineKeyboard);
        }
        else
        {
            var conversation = await _conversationService.GetAddProductConversation(message.Chat.Id)!;
            if (conversation is not null) await _conversationService.DeleteAddProductConversation(conversation);
            
            
            await client.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Choose action",
            replyMarkup: KeyboardMarkups.MenuKeyboardMarkup);
        }
    }
}