using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Telegram.Bot.Types;
using TrackCaloriesBot.Context;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Enums;
using TrackCaloriesBot.Service.Interfaces;

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
        await _mealDataService.AddNewProduct(newProduct, update);

        return result.Entity;
    }

    public async Task<Product?>? GetProduct(long? id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
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
            var check = int.TryParse(message, out var x);
            if (check)
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
            var check = int.TryParse(message, out var x);
            if (check)
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
            var check = int.TryParse(message, out var x);
            if (check)
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
            var check = int.TryParse(message, out var x);
            if (check)
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
            var check = int.TryParse(message, out var x);
            if (check)
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
            var check = int.TryParse(message, out var x);
            if (check)
            {
                product.Result.Quantity = x;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddProductInfoFromResponse(ResponseProduct responseProduct, long? id)
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