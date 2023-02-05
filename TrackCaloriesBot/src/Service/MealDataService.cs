using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TrackCaloriesBot.Context;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Enums;

namespace TrackCaloriesBot.Service;

public class MealDataService : IMealDataService
{
    private readonly ApplicationDbContext _context;

    public MealDataService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<MealData> AddNewMealData(Update update)
    {
        var type = update.Message.Text switch
        {
            "Breakfast" => MealType.Breakfast,
            "Lunch" => MealType.Lunch,
            "Dinner" => MealType.Dinner,
            "Snack" => MealType.Snack
        };
        
        var messageDate = update.Message.Date.ToString("dd.MM.yyyy");
        var dayTotalData = await _context.DayTotalData.FirstOrDefaultAsync(x =>
            x.Date == messageDate);
        
        var mealData = await _context.MealData.FirstOrDefaultAsync(x =>
                    dayTotalData != null && x.DayTotalData.DayId == dayTotalData.DayId && x.MealType == type);

        if (mealData != null) return mealData;

        var newMealData = new MealData
        {
            MealId = Guid.NewGuid(),
            MealType = type,
            DayTotalData = dayTotalData,
            Products = null
        };

        dayTotalData?.MealData?.Add(newMealData);
        var result = await _context.MealData.AddAsync(newMealData);
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<MealData?> GetMealData(Update update)
    {
        var type = update.Message.Text switch
        {
            "Breakfast" => MealType.Breakfast,
            "Lunch" => MealType.Lunch,
            "Dinner" => MealType.Dinner,
            "Snack" => MealType.Snack
        };
        
        var messageDate = update.Message.Date.ToString("dd.MM.yyyy");
        var dayTotalData = await _context.DayTotalData.FirstOrDefaultAsync(x =>
            x.Date == messageDate);

        var mealData = await _context.MealData.FirstOrDefaultAsync(x =>
            dayTotalData != null && x.DayTotalData.DayId == dayTotalData.DayId && x.MealType == type);
        
        return mealData;
    }

    public Task AddNewProduct(Update update)
    {
        throw new NotImplementedException();
    }
}