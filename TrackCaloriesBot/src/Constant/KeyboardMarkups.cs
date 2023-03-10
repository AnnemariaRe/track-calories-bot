using Telegram.Bot.Types.ReplyMarkups;

namespace TrackCaloriesBot.Constant;

public static class KeyboardMarkups
{
    public static ReplyKeyboardMarkup MenuKeyboardMarkup = new(new[]
    {
        new KeyboardButton[] { "New record", "Recipes" },
        new KeyboardButton[] { "My progress", "Info about me" },
        new KeyboardButton[] { "Summary for today" }
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
        new KeyboardButton[] { "Get from my saved ones" },
        new KeyboardButton[] { "Back" }
    }){
        ResizeKeyboard = true
    };
    public static ReplyKeyboardMarkup RecipesKeyboardMarkup = new(new[]
    {
        new KeyboardButton[] { "Search new recipes", "Create my own recipe" },
        new KeyboardButton[] { "Show my recipes" },
        new KeyboardButton[] { "Back" },
    }){
        ResizeKeyboard = true
    };
    public static ReplyKeyboardMarkup AfterRecipeInfoKeyboardMarkup = new(new[]
    {
        new KeyboardButton[] { "Save to my favourites" },
        new KeyboardButton[] { "Get another recipe with same properties", "Get another recipe with new properties" },
        new KeyboardButton[] { "Back" },
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
    public static ReplyKeyboardMarkup GreatKeyboardMarkup = new(new[]
    {
        new KeyboardButton[] { "Great!" },
    }){
        ResizeKeyboard = true
    };
}