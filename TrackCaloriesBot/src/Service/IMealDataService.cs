using Telegram.Bot.Types;
using TrackCaloriesBot.Entity;

namespace TrackCaloriesBot.Service;

public interface IMealDataService
{
    public Task<MealData> AddNewMealData(Update update);
    public Task<MealData?> GetMealData(Update update);
    public Task AddNewProduct(Update update);
}