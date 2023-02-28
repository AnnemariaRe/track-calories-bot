using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TrackCaloriesBot.Context;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Enums;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Repository;

public class DayTotalDataRepo : IDayTotalDataRepo
{
    private readonly ApplicationDbContext _context;
    private readonly IMealDataRepo _mealDataRepo;
    private readonly IUserRepo _userRepo;

    public DayTotalDataRepo(ApplicationDbContext context, IMealDataRepo mealDataRepo, IUserRepo userRepo)
    {
        _context = context;
        _mealDataRepo = mealDataRepo;
        _userRepo = userRepo;
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
            MealData = new List<MealData>(),
            Water = 0,
            Date = messageDate
        };

        await _userRepo.AddDayTotalData(dayTotalData, update);
        
        var result = await _context.DayTotalData.AddAsync(newDayTotalData);
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<DayTotalData?>? GetDayTotalData(Update update)
    {
        var messageDate = update.Message.Date.ToString("dd.MM.yyyy");
        
        var dayTotalData = await _context.DayTotalData.FirstOrDefaultAsync(x =>
                x.Date == messageDate);

        return dayTotalData;
    }

    public async Task<MealData?> GetMealData(MealType mealType, DayTotalData dayTotalData)
    { 
        var mealData = _context.MealData.FirstOrDefault(x =>
            x.DayTotalData != null && x.DayTotalData.DayId == dayTotalData.DayId && x.MealType == mealType);

        return mealData;
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
                dayTotalData.Result.MealData.Add(_mealDataRepo.AddNewMealData(update).Result);
            } 
            if (dayTotalData.Result.MealData.FirstOrDefault(x => x.MealType == MealType.Lunch) == null &&
                update.Message.Text == "Lunch")
            {
                dayTotalData.Result.MealData.Add(_mealDataRepo.AddNewMealData(update).Result);
            }
            if (dayTotalData.Result.MealData.FirstOrDefault(x => x.MealType == MealType.Dinner) == null &&
                update.Message.Text == "Dinner")
            {
                dayTotalData.Result.MealData.Add(_mealDataRepo.AddNewMealData(update).Result);
            }
            if (dayTotalData.Result.MealData.FirstOrDefault(x => x.MealType == MealType.Snack) == null &&
                update.Message.Text == "Snack")
            {
                dayTotalData.Result.MealData.Add(_mealDataRepo.AddNewMealData(update).Result);
            }
        }
        await _context.SaveChangesAsync();
    }
}