using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TrackCaloriesBot.Context;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Enums;
using TrackCaloriesBot.Service.Interfaces;

namespace TrackCaloriesBot.Service;

public class DayTotalDataService : IDayTotalDataService
{
    private readonly ApplicationDbContext _context;
    private readonly IMealDataService _mealDataService;
    private readonly IUserService _userService;

    public DayTotalDataService(ApplicationDbContext context, IMealDataService mealDataService, IUserService userService)
    {
        _context = context;
        _mealDataService = mealDataService;
        _userService = userService;
    }
    
    public async Task<DayTotalData> AddNewDayTotalData(Update update)
    {
        var messageDate = update.Message.Date.ToString("dd.MM.yyyy");

        var dayTotalData = await _context.DayTotalData.FirstOrDefaultAsync(x =>
            x.Date == messageDate);

        if (dayTotalData != null) return dayTotalData;

        var newDayTotalData = new DayTotalData
        {
            DayId = messageDate.GetHashCode(),
            MealData = null,
            Water = 0,
            Date = messageDate
        };

        await _userService.AddDayTotalData(dayTotalData, update);
        
        var result = await _context.DayTotalData.AddAsync(newDayTotalData);
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<DayTotalData> GetDayTotalData(Update update)
    {
        var messageDate = update.Message.Date.ToString("dd.MM.yyyy");
        
        var dayTotalData = await _context.DayTotalData.FirstOrDefaultAsync(x =>
                x.Date == messageDate);
        
        return dayTotalData;
    }

    public async Task AddWater(Update update, float x)
    {
        var dayTotalData = GetDayTotalData(update);
        dayTotalData.Result.Water += x; 
        await _context.SaveChangesAsync();
    }

    public async Task AddNewMealType(Update update)
    {
        var dayTotalData = GetDayTotalData(update);
        if (dayTotalData.Result is not null)
        {
            if (dayTotalData.Result.MealData.FirstOrDefault(x => x.MealType == MealType.Breakfast) == null &&
                update.Message.Text == "Breakfast")
            {
                dayTotalData.Result.MealData.Add(_mealDataService.AddNewMealData(update).Result);
            } 
            if (dayTotalData.Result.MealData.FirstOrDefault(x => x.MealType == MealType.Lunch) == null &&
                update.Message.Text == "Lunch")
            {
                dayTotalData.Result.MealData.Add(_mealDataService.AddNewMealData(update).Result);
            }
            if (dayTotalData.Result.MealData.FirstOrDefault(x => x.MealType == MealType.Dinner) == null &&
                update.Message.Text == "Dinner")
            {
                dayTotalData.Result.MealData.Add(_mealDataService.AddNewMealData(update).Result);
            }
            if (dayTotalData.Result.MealData.FirstOrDefault(x => x.MealType == MealType.Snack) == null &&
                update.Message.Text == "Snack")
            {
                dayTotalData.Result.MealData.Add(_mealDataService.AddNewMealData(update).Result);
            }
        }
        await _context.SaveChangesAsync();
    }
}