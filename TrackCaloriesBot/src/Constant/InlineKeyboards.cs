using Telegram.Bot.Types.ReplyMarkups;

namespace TrackCaloriesBot.Constant;

public static class InlineKeyboards
{
    public static InlineKeyboardMarkup StartInlineKeyboard = new InlineKeyboardMarkup(new[]
    { 
        new[] { InlineKeyboardButton.WithCallbackData("Register", "/register") },
    });
    public static InlineKeyboardMarkup UpdateInlineKeyboard = new InlineKeyboardMarkup(new[]
    {
        new[] { InlineKeyboardButton.WithCallbackData("Show user info", "/show") },
    });
    
}