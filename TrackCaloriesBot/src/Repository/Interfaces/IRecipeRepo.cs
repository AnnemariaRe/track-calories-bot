using Telegram.Bot.Types;
using TrackCaloriesBot.Entity;

namespace TrackCaloriesBot.Repository.Interfaces;

public interface IRecipeRepo
{
    public Task<Recipe> CreateRecipe(Update update, long id);
    public Task<Recipe?> GetRecipe(long? id);
    public Task AddServingsNumber(string message, long? id);
    public Task AddCalorieAmount(string? message, long? id);
    public Task AddProtein(string? message, long? id);
    public Task AddFat(string? message, long? id);
    public Task AddCarbs(string? message, long? id);
    public Task AddReadyInMinutes(string? message, long? id);
    public Task AddWeightPerServing(string? message, long? id);
    public Task AddProduct(Product? product, long? id);
    public Task AddProducts(ICollection<Product>? products, long? id);
    public Task CreateRecipeFromResponse(ResponseRecipe response, long id);
    public Task DeleteRecipe(long? id);
}