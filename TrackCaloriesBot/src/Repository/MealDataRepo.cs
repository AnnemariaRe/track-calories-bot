using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TrackCaloriesBot.Context;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Enums;
using TrackCaloriesBot.Exceptions;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Repository;

public class MealDataRepo : IMealDataRepo
{
    private readonly ApplicationDbContext _context;

    public MealDataRepo(ApplicationDbContext context)
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
            MealId = Guid.NewGuid().GetHashCode(),
            MealType = type,
            DayTotalData = dayTotalData,
            Products = null
        };

        dayTotalData?.MealData?.Add(newMealData);
        var result = await _context.MealData.AddAsync(newMealData);
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<MealData?> GetMealData(Update update, MealType? mealType)
    {
        var messageDate = update?.Message?.Date.ToString("dd.MM.yyyy");
        var dayTotalData = await _context.DayTotalData.FirstOrDefaultAsync(x =>
            x.Date == messageDate);

        if (dayTotalData is null)
        {
            throw new NullBotException("User entity is not found.");
        }

        var mealData = await _context.MealData.FirstOrDefaultAsync(x =>
            x.DayTotalData != null && x.DayTotalData.DayId == dayTotalData.DayId && x.MealType == mealType);
        
        return mealData;
    }

    public async Task AddNewProduct(Product? product, Update update)
    {
        if (product != null)
        {
            var mealData = await GetMealData(update, product.MealData.MealType);
            mealData?.Products?.Add(product);
            await _context.SaveChangesAsync();
        }
    }

    public List<Product> GetProducts(MealData mealData)
    { 
        return _context.Products.Where(x => x.MealData == mealData).ToList();
    }
}