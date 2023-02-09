using Telegram.Bot.Types;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Enums;

namespace TrackCaloriesBot.Service.Interfaces;

public interface IDayTotalDataService
{
    public Task<DayTotalData> AddNewDayTotalData(Update update);
    public Task<DayTotalData?>? GetDayTotalData(Update update);
    public Task<MealData?> GetMealData(MealType mealType, DayTotalData dayTotalData);
    public Task AddWater(Update update, float x);
    public Task AddNewMealType(Update update);
}