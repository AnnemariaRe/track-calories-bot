using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TrackCaloriesBot.Context;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Enums;
using TrackCaloriesBot.Service.Interfaces;
using static TrackCaloriesBot.Enums.MealType;

namespace TrackCaloriesBot.Service;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    private readonly IMealDataService _mealDataService;

    public ProductService(ApplicationDbContext context, IMealDataService mealDataService)
    {
        _context = context;
        _mealDataService = mealDataService;
    }
    public async Task<Product> CreateProduct(Update update)
    {
        var conversation =
            await _context.ProductConversations.FirstOrDefaultAsync( x => 
                update.Message != null && x.User.TgId == update.Message.Chat.Id);

        var mealType = conversation?.MealType switch
        {
            "Breakfast" => MealType.Breakfast,
            "Dinner" => MealType.Dinner,
            "Lunch" => MealType.Lunch,
            "Snack" => MealType.Snack
        };
        var mealData = await _mealDataService.GetMealData(update, mealType);
        
        var product = await _context.Products.FirstOrDefaultAsync(
            x => update.Message != null && x.ProductId == conversation!.ProductId);
        
        if (product != null) return product;
        
        var newProduct = new Product()
        {
            ProductId = new Guid().GetHashCode(),
            Name = update.Message?.Text,
            Quantity = 1,
            MealData = mealData
        };

        await _mealDataService.AddNewProduct(newProduct, update);
        var result = await _context.Products.AddAsync(newProduct);
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<Product?>? GetProduct(long id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == id);
        return product;
    }

    public async Task AddServingUnit(Update update, long id)
    {
        var product = GetProduct(id);
        if (product?.Result is not null)
        {
            product.Result.ServingType = update.CallbackQuery?.Data switch
                {
                    "Grams" => ServingType.Grams,
                    "Milliliters" => ServingType.Milliliters,
                };
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddServingAmount(Update update, long id)
    {
        var product = GetProduct(id);
        if (product?.Result is not null)
        {
            var check = int.TryParse(update.Message?.Text, out var x);
            if (check)
            {
                product.Result.ServingAmount = x;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddCalorieAmount(Update update, long id)
    {
        var product = GetProduct(id);
        if (product?.Result is not null)
        {
            var check = int.TryParse(update.Message?.Text, out var x);
            if (check)
            {
                product.Result.BaseCalories = x;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddProtein(Update update, long id)
    {
        var product = GetProduct(id);
        if (product?.Result is not null)
        {
            var check = int.TryParse(update.Message?.Text, out var x);
            if (check)
            {
                product.Result.BaseProtein = x;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddFat(Update update, long id)
    {
        var product = GetProduct(id);
        if (product?.Result is not null)
        {
            var check = int.TryParse(update.Message?.Text, out var x);
            if (check)
            {
                product.Result.BaseFat = x;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddCarbs(Update update, long id)
    {
        var product = GetProduct(id);
        if (product?.Result is not null)
        {
            var check = int.TryParse(update.Message?.Text, out var x);
            if (check)
            {
                product.Result.BaseCarbs = x;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddQuantity(Update update, long id)
    {
        var product = GetProduct(id);
        if (product?.Result is not null)
        {
            var check = int.TryParse(update.Message?.Text, out var x);
            if (check)
            {
                product.Result.Quantity = x;
                await _context.SaveChangesAsync();
            }
        }
    }
}