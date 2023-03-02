using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;
using Telegram.Bot.Types;
using TrackCaloriesBot.Context;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Enums;
using TrackCaloriesBot.Exceptions;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Repository;

public class ProductRepo : IProductRepo
{
    private readonly ApplicationDbContext _context;
    private readonly IConnectionMultiplexer _redis;
    private readonly IMealDataRepo _mealDataRepo;

    public ProductRepo(ApplicationDbContext context, IMealDataRepo mealDataRepo, IConnectionMultiplexer redis)
    {
        _context = context;
        _mealDataRepo = mealDataRepo;
        _redis = redis;
    }
    public async Task<Product> CreateProduct(Update update)
    {
        var db = _redis.GetDatabase();
        var redisConveration = db.StringGet(update.Message.Chat.Id.ToString());
        var conversation = JsonConvert.DeserializeObject<ConversationData>(redisConveration);

        var mealType = conversation?.MealType switch
        {
            "Breakfast" => MealType.Breakfast,
            "Dinner" => MealType.Dinner,
            "Lunch" => MealType.Lunch,
            "Snack" => MealType.Snack
        };
        var mealData = await _mealDataRepo.GetMealData(update, mealType);
        
        var product = await _context.Products.FirstOrDefaultAsync(
        x => update.Message != null && x.Name == update.Message.Text);

        if (product != null) return product;

        var newProduct = new Product()
        {
            Id = new Guid().GetHashCode(),
            Name = update.Message?.Text,
            Quantity = 1,
            MealData = mealData,
            BaseProtein = 0,
            BaseFat = 0,
            BaseCarbs = 0
        };

        var result = await _context.Products.AddAsync(newProduct);
        await _context.SaveChangesAsync();
        await _mealDataRepo.AddNewProduct(newProduct, update);

        return result.Entity;
    }

    public async Task<Product?> GetProduct(long? id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
        if (product is null)
        {
            throw new NullBotException("Product entity is not found.");
        }
        return product;
    }

    public async Task AddServingUnit(string message, long? id)
    {
        var product = GetProduct(id);
        if (product?.Result is not null)
        {
            product.Result.ServingType = message switch
                {
                    "Grams" => ServingType.Grams,
                    "Milliliters" => ServingType.Milliliters,
                    _ => throw new ArgumentOutOfRangeException()
                };
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task AddName(string message, long? id)
    {
        var product = GetProduct(id);
        if (product?.Result is not null)
        {
            product.Result.Name = message;
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddServingAmount(string message, long? id)
    {
        var product = GetProduct(id);
        if (product?.Result is not null)
        {
            if (int.TryParse(message, out var x))
            {
                product.Result.ServingAmount = x;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddCalorieAmount(string? message, long? id)
    {
        var product = GetProduct(id);
        if (product?.Result is not null)
        {
            if (int.TryParse(message, out var x))
            {
                product.Result.BaseCalories = x;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddProtein(string? message, long? id)
    {
        var product = GetProduct(id);
        if (product?.Result is not null)
        {
            if (int.TryParse(message, out var x))
            {
                product.Result.BaseProtein = x;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddFat(string? message, long? id)
    {
        var product = GetProduct(id);
        if (product?.Result is not null)
        {
            if (int.TryParse(message, out var x))
            {
                product.Result.BaseFat = x;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddCarbs(string? message, long? id)
    {
        var product = GetProduct(id);
        if (product?.Result is not null)
        {
            if (int.TryParse(message, out var x))
            {
                product.Result.BaseCarbs = x;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddQuantity(string message, long? id)
    {
        var product = GetProduct(id);
        if (product?.Result is not null)
        {
            if (int.TryParse(message, out var x))
            {
                product.Result.Quantity = x;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddProductInfoFromResponse(ResponseItem responseProduct, long? id)
    {
        var product = GetProduct(id);
        if (product?.Result is not null)
        {
            await AddName(responseProduct.Title, id);
            await AddCalorieAmount(
                ((int)responseProduct.Nutrition.Nutrients.FirstOrDefault(x => x.Name == "Calories")?.Amount!)
                .ToString(), id);
            await AddCarbs(((int)responseProduct.Nutrition.Nutrients.FirstOrDefault(x => x.Name == "Carbohydrates")
                                           ?.Amount!).ToString(), id);
            await AddProtein(
                ((int)responseProduct.Nutrition.Nutrients.FirstOrDefault(x => x.Name == "Protein")?.Amount!).ToString(),
                id);
            await AddFat(
                ((int)responseProduct.Nutrition.Nutrients.FirstOrDefault(x => x.Name == "Fat")?.Amount!).ToString(),
                id);
            await AddServingUnit("Grams", id);
        }
    }
    
    public async Task DeleteProduct(long? id)
    {
        var product = await GetProduct(id)!;
        _context.Products.Remove(product!);
        await _context.SaveChangesAsync();
    }
}