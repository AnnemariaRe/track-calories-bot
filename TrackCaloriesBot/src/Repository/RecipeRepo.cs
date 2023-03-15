using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Telegram.Bot.Types;
using TrackCaloriesBot.Context;
using TrackCaloriesBot.Entity;
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

    public async Task<Recipe?> CreateRecipe(Update update)
    {
        var recipe = await _context.Recipes.FirstOrDefaultAsync(
            x => update.Message != null && x.Name == update.Message.Text && x.Id == update.Message.Chat.Id);
        
        if (recipe != null) return recipe;
        
        var newRecipe = new Recipe()
        {
            Id = Guid.NewGuid().GetHashCode(),
            UserId = update.Message.Chat.Id,
            Name = update.Message?.Text,
            Image = null,
            SourceUrl = null,
            ServingsNumber = 0,
            ReadyInMinutes = 0,
            WeightPerServing = 0,
            BaseProtein = 0,
            BaseFat = 0,
            BaseCarbs = 0,
            Products = new List<Product>(),
            Description = null
        };
        
        var result = await _context.Recipes.AddAsync(newRecipe);
        await _userRepo.AddRecipe(update.Message.Chat.Id, newRecipe);
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<Recipe?> GetRecipe(int? id)
    {
        var recipe = await _context.Recipes.FirstOrDefaultAsync(x => x.Id == id);
        if (recipe is null)
        {
            throw new NullBotException("Recipe entity is not found.");
        }
        return recipe;
    }

    public async Task AddServingsNumber(string? message, int? id)
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

    public async Task AddAllCalories(int? id)
    {
        var recipe = GetRecipe(id).Result;
        if (recipe is not null && recipe.Products is not null)
        {
            double totalCalories = 0;
            double totalWeight = 0;
            foreach (var product in recipe.Products)
            {
                totalCalories += product.BaseCalories / 100 * product.ServingAmount * product.Quantity;
                totalWeight += product.ServingAmount * product.Quantity;
            }
            recipe.BaseCalories = totalCalories / totalWeight * (totalWeight / recipe.ServingsNumber);
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddPFC(int? id)
    {
        var recipe = GetRecipe(id).Result;
        if (recipe is not null && recipe.Products is not null)
        {
            double protein = 0;
            double fat = 0;
            double carbs = 0;
            double totalWeight = 0;
            foreach (var product in recipe.Products)
            {
                protein += product.BaseProtein / 100 * product.ServingAmount * product.Quantity;
                fat += product.BaseFat / 100 * product.ServingAmount * product.Quantity;
                carbs += product.BaseCarbs / 100 * product.ServingAmount * product.Quantity;
                totalWeight += product.ServingAmount * product.Quantity;
            }
            recipe.BaseProtein = protein / totalWeight * (totalWeight / recipe.ServingsNumber);
            recipe.BaseFat = fat / totalWeight * (totalWeight / recipe.ServingsNumber);
            recipe.BaseCarbs = carbs / totalWeight * (totalWeight / recipe.ServingsNumber);
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddReadyInMinutes(string? message, int? id)
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

    public async Task AddWeightPerServing(int? id)
    {
        var recipe = GetRecipe(id).Result;
        if (recipe is not null && recipe.Products is not null)
        {
            double totalWeight = 0;
            foreach (var product in recipe.Products)
            {
                totalWeight += product.ServingAmount * product.Quantity;
            }

            recipe.WeightPerServing = totalWeight / recipe.ServingsNumber;
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddProduct(Product? product, int? id)
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

    public async Task AddProducts(ICollection<Product>? products, int? id)
    {
        if (products is not null)
        {
            foreach (var product in products)
            {
                await AddProduct(product, id);
            }
        }
    }
    
    public async Task AddDescription(string? message, int? id)
    {
        var recipe = GetRecipe(id);
        if (recipe?.Result is not null)
        {
            recipe.Result.Description = message;
            await _context.SaveChangesAsync();
        }
    }

    public async Task CreateRecipeFromResponse(ResponseRecipe? response, long id)
    {
        if (response is null) throw new NullBotException("Recipe response cannot be null");
        
        var recipe = await _context.Recipes.FirstOrDefaultAsync(
            x => x.Name == response.Title && x.ApiId == response.Id);
        
        if (recipe != null) throw new BotException("Recipe entity already exists");

        ICollection<Product>? ingredients = new List<Product>();
        foreach (var ingredient in response.Ingredients)
        {
            ingredients?.Add(new Product
            {
                Id = Guid.NewGuid().GetHashCode(),
                Name = ingredient.OriginalName,
                ServingType = ingredient.ServingType,
                ServingAmount = ingredient.ServingAmount,
                Quantity = 0,
                BaseCalories = 0,
                BaseProtein = 0,
                BaseFat = 0,
                BaseCarbs = 0,
                MealData = null,
            });
        }
        
        var newRecipe = new Recipe()
        {
            Id = Guid.NewGuid().GetHashCode(),
            UserId = id,
            ApiId = response.Id,
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
            Products = ingredients,
            Description = null
        };
        
        await _context.Recipes.AddAsync(newRecipe);
        await _userRepo.AddRecipe(id, newRecipe);
        
        await _context.SaveChangesAsync();
    }
    
    public async Task<ICollection<Recipe>?> GetAllRecipes(long? id)
    {
        var user = _context.Recipes.Where(x => x.UserId == id).ToList();
        if (user is null) throw new NullBotException("User entity is not found.");

        return user;
    }

    public async Task DeleteRecipe(int? id)
    {
        var recipe = await GetRecipe(id);
        if (recipe != null)
            _context.Recipes.Remove(_context.Recipes.Include(p => p.Products).FirstOrDefault(x => x.Id == id));
        await _context.SaveChangesAsync();

        }
}