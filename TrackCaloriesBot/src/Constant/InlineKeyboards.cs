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
    public static InlineKeyboardMarkup ServingTypeInlineKeyboard = new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("grams", "Grams"),
            InlineKeyboardButton.WithCallbackData("milliliters", "Milliliters")
        },
    });
    public static InlineKeyboardMarkup YesOrNoInlineKeyboard = new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("yes", "yes"),
            InlineKeyboardButton.WithCallbackData("no", "no")
        },
    });
    
}