using Telegram.Bot.Types;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Enums;

namespace TrackCaloriesBot.Service.Interfaces;

public interface IMealDataService
{
    public Task<MealData> AddNewMealData(Update update);
    public Task<MealData?> GetMealData(Update update, MealType mealType);
    public Task AddNewProduct(Product? product, Update update);
}