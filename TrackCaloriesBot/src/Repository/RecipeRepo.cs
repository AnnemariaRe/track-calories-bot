
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TrackCaloriesBot.Context;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Enums;
using TrackCaloriesBot.Exceptions;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Repository;

public class RecipeRepo : IRecipeRepo
{
    private readonly ApplicationDbContext _context;
    private readonly IUserRepo _userRepo;

    public RecipeRepo(ApplicationDbContext context, IUserRepo userRepo)
    {
        _context = context;
        _userRepo = userRepo;
    }

    public async Task<Recipe> CreateRecipe(Update update, long id)
    {
        var recipe = await _context.Recipes.FirstOrDefaultAsync(
            x => update.Message != null && x.Name == update.Message.Text && x.Id == id);
        
        if (recipe != null) return recipe;
        
        var newRecipe = new Recipe()
        {
            Id = new Guid().GetHashCode(),
            Name = update.Message?.Text,
            Image = null,
            SourceUrl = null,
            ServingsNumber = 0,
            ReadyInMinutes = 0,
            WeightPerServing = 0,
            BaseProtein = 0,
            BaseFat = 0,
            BaseCarbs = 0,
        };

        await _userRepo.AddRecipe(update.Message.Chat.Id, newRecipe);
        var result = await _context.Recipes.AddAsync(newRecipe);
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<Recipe?> GetRecipe(long? id)
    {
        var recipe = await _context.Recipes.FirstOrDefaultAsync(x => x.Id == id);
        if (recipe is null)
        {
            throw new NullBotException("Recipe entity is not found.");
        }
        return recipe;
    }

    public async Task AddServingsNumber(string message, long? id)
    {
        var recipe = GetRecipe(id);
        if (recipe?.Result is not null)
        {
            if (int.TryParse(message, out var x)) { 
                recipe.Result.ServingsNumber = x;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddCalorieAmount(string? message, long? id)
    {
        var recipe = GetRecipe(id);
        if (recipe?.Result is not null)
        {
            if (double.TryParse(message, out var x)) { 
                recipe.Result.BaseCalories = x;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddProtein(string? message, long? id)
    {
        var recipe = GetRecipe(id);
        if (recipe?.Result is not null)
        {
            if (double.TryParse(message, out var x)) { 
                recipe.Result.BaseProtein = x;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddFat(string? message, long? id)
    {
        var recipe = GetRecipe(id);
        if (recipe?.Result is not null)
        {
            if (double.TryParse(message, out var x)) { 
                recipe.Result.BaseFat = x;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddCarbs(string? message, long? id)
    {
        var recipe = GetRecipe(id);
        if (recipe?.Result is not null)
        {
            if (double.TryParse(message, out var x)) { 
                recipe.Result.BaseCarbs = x;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddReadyInMinutes(string? message, long? id)
    {
        var recipe = GetRecipe(id);
        if (recipe?.Result is not null)
        {
            if (message.Contains('.')) message.Remove(message.IndexOf('.'));
            if (int.TryParse(message, out var x)) { 
                recipe.Result.ReadyInMinutes = x;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddWeightPerServing(string? message, long? id)
    {
        var recipe = GetRecipe(id);
        if (recipe?.Result is not null)
        {
            if (message.Contains('.')) message.Remove(message.IndexOf('.'));
            if (int.TryParse(message, out var x)) { 
                recipe.Result.WeightPerServing = x;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddProduct(Product? product, long? id)
    {
        var recipe = GetRecipe(id);
        if (recipe?.Result is not null && product is not null)
        {
            if (!recipe.Result.Products.Contains(product))
            {
                recipe.Result.Products.Add(product);
            }
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddProducts(ICollection<Product>? products, long? id)
    {
        if (products is not null)
        {
            foreach (var product in products)
            {
                await AddProduct(product, id);
            }
        }
    }

    public async Task CreateRecipeFromResponse(ResponseRecipe response, long id)
    {
        if (response is null) throw new BotException("Recipe response cannot be null");
        
        var recipe = await _context.Recipes.FirstOrDefaultAsync(
            x => x.Name == response.Title && x.ApiId == response.Id);
        
        if (recipe != null) throw new BotException("Recipe entity already exists");

        ICollection<Product>? ingredients = null;
        foreach (var ingredient in response.Ingredients)
        {
            ingredients?.Add(new Product
            {
                Id = 0,
                Name = ingredient.Name,
                ServingType = ServingType.Grams,
                ServingAmount = 0,
                Quantity = 0,
                BaseCalories = ingredient.Nutrition.Nutrients.FirstOrDefault(x => x.Name == "Calories")!.Amount,
                BaseProtein = ingredient.Nutrition.Nutrients.FirstOrDefault(x => x.Name == "Protein")!.Amount,
                BaseFat = ingredient.Nutrition.Nutrients.FirstOrDefault(x => x.Name == "Fat")!.Amount,
                BaseCarbs = ingredient.Nutrition.Nutrients.FirstOrDefault(x => x.Name == "Carbohydrates")!.Amount,
                MealData = null
            });
        }
        
        var newRecipe = new Recipe()
        {
            Id = new Guid().GetHashCode(),
            Name = response.Title,
            Image = response.Image,
            SourceUrl = response.SourceUrl,
            ServingsNumber = response.Servings,
            ReadyInMinutes = response.ReadyInMinutes,
            WeightPerServing = response.WeightPerServing,
            BaseCalories = response.Nutrition.Nutrients.FirstOrDefault(x => x.Name == "Calories")!.Amount,
            BaseProtein = response.Nutrition.Nutrients.FirstOrDefault(x => x.Name == "Protein")!.Amount,
            BaseFat = response.Nutrition.Nutrients.FirstOrDefault(x => x.Name == "Fat")!.Amount,
            BaseCarbs = response.Nutrition.Nutrients.FirstOrDefault(x => x.Name == "Carbohydrates")!.Amount,
            Products = ingredients
        };
        
        await _userRepo.AddRecipe(id, newRecipe);
        await _context.Recipes.AddAsync(newRecipe);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRecipe(long? id)
    {
        var recipe = await GetRecipe(id)!;
        _context.Recipes.Remove(recipe!);
        await _context.SaveChangesAsync();
    }
}