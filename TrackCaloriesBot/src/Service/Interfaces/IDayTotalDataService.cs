using Telegram.Bot.Types;
using TrackCaloriesBot.Entity;

namespace TrackCaloriesBot.Service.Interfaces;

public interface IDayTotalDataService
{
    public Task<DayTotalData> AddNewDayTotalData(Update update);
    public Task<DayTotalData?> GetDayTotalData(Update update);
    public Task AddWater(Update update, float x);
    public Task AddNewMealType(Update update);
}