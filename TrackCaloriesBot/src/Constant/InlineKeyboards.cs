using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TrackCaloriesBot.Constant;

public static class InlineKeyboards
{
    public static InlineKeyboardMarkup StartInlineKeyboard = new InlineKeyboardMarkup(new[]
    { 
        new[] { InlineKeyboardButton.WithCallbackData("Register", "/register") },
    });
    public static InlineKeyboardMarkup ServingTypeInlineKeyboard = new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("grams", "Grams"),
            InlineKeyboardButton.WithCallbackData("milliliters", "Milliliters")
        },
    });
    public static InlineKeyboardMarkup AddIngredientInlineKeyboard = new InlineKeyboardMarkup(new[]
    {
        new[] { InlineKeyboardButton.WithCallbackData("Add ingredient", "/add_ingredient") },
    });
    public static InlineKeyboardMarkup AddIngredientFinishInlineKeyboard = new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Add ingredient", "/add_ingredient"),
            InlineKeyboardButton.WithCallbackData("Finish", "/finish")
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
    public static InlineKeyboardMarkup SearchInlineKeyboard = new(new[]
    {
        InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(
            text: "start searching...")
    });
    public static InlineKeyboardMarkup SearchRecipeInlineKeyboard = new(new[]
    {
        InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(
            text: "start searching...")
    });
    public static InlineKeyboardMarkup ProductInfoInlineKeyboard = new InlineKeyboardMarkup(new[]
    {
        new[] { InlineKeyboardButton.WithCallbackData("Continue", "/continue") },
        new[] { InlineKeyboardButton.WithCallbackData("Choose another one", "/search") }
    });
    public static InlineKeyboardMarkup SkipInlineKeyboard = new InlineKeyboardMarkup(new[]
    {
        new[] { InlineKeyboardButton.WithCallbackData("Skip", "/skip") },
    });

}