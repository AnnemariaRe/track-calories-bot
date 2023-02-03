using Telegram.Bot.Types.ReplyMarkups;

namespace TrackCaloriesBot.Constant;

public static class KeyboardMarkups
{
    public static ReplyKeyboardMarkup MenuKeyboardMarkup = new(new[]
    {
        new KeyboardButton[] { "New record", "Summary for today" },
        new KeyboardButton[] { "My progress", "Info about me" },
    }){
        ResizeKeyboard = true
    };
    public static ReplyKeyboardMarkup NewRecordKeyboardMarkup = new(new[]
    {
        new KeyboardButton[] { "Breakfast", "Lunch" },
        new KeyboardButton[] { "Dinner", "Snack" },
        new KeyboardButton[] { "Water" },
        new KeyboardButton[] { "Back" }
    }){
        ResizeKeyboard = true
    };
    public static ReplyKeyboardMarkup MealKeyboardMarkup = new(new[]
    {
        new KeyboardButton[] { "Search products", "Enter manually" },
        new KeyboardButton[] { "Scan a barcode", "Get from your favourites" },
        new KeyboardButton[] { "Back" }
    }){
        ResizeKeyboard = true
    };
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