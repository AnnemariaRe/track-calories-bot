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
        var messageDate = update.Message.Date;
        var type = update.Message.Text switch
        {
            "Breakfast" => MealType.Breakfast,
            "Lunch" => MealType.Lunch,
            "Dinner" => MealType.Dinner,
            "Snack" => MealType.Snack
        };
        
        var dayTotalData = await _context.DayTotalData.FirstOrDefaultAsync(x =>
            x.Date.ToShortDateString() == messageDate.ToShortDateString());

        var mealData = await _context.MealData.FirstOrDefaultAsync(x =>
            x.dayId == dayTotalData.dayId && x.MealType == type);
        
        if (mealData != null) return mealData;

        var newMealData = new MealData
        {
            mealId = Guid.NewGuid(),
            dayId = dayTotalData.dayId,
            MealType = type,
            DayTotalData = dayTotalData,
            Products = null
        };
        
        var result = await _context.MealData.AddAsync(newMealData);
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<MealData?> GetMealData(Update update)
    {
        var messageDate = update.Message.Date;
        var type = update.Message.Text switch
        {
            "Breakfast" => MealType.Breakfast,
            "Lunch" => MealType.Lunch,
            "Dinner" => MealType.Dinner,
            "Snack" => MealType.Snack
        };
        
        var dayTotalData = await _context.DayTotalData.FirstOrDefaultAsync(x =>
            x.Date.ToShortDateString() == messageDate.ToShortDateString());

        var mealData = await _context.MealData.FirstOrDefaultAsync(x =>
            x.dayId == dayTotalData.dayId && x.MealType == type);

        return mealData;
    }

    public Task AddNewProduct(Update update)
    {
        throw new NotImplementedException();
    }
}