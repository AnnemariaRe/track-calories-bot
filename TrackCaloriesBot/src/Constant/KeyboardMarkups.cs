using Telegram.Bot.Types.ReplyMarkups;

namespace TrackCaloriesBot.Constant;

public static class KeyboardMarkups
{
    public static ReplyKeyboardMarkup GenderKeyboardMarkup = new(new[]
    {
        new KeyboardButton[] { "Female", "Male️" },
    });
    public static ReplyKeyboardMarkup GoalKeyboardMarkup = new(new[]
    {
        new KeyboardButton[] { "Lose weight" },
        new KeyboardButton[] { "Maintain weight" },
        new KeyboardButton[] { "Gain weight" },
    });
    public static ReplyKeyboardMarkup ActivityKeyboardMarkup = new(new[]
    {
        new KeyboardButton[] { "Low" },
        new KeyboardButton[] { "Moderate" },
        new KeyboardButton[] { "High" },
        new KeyboardButton[] { "Very high" },
    });
}