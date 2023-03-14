using Telegram.Bot.Types;
using TrackCaloriesBot.Entity;

namespace TrackCaloriesBot.Repository.Interfaces;

public interface IRecipeRepo
{
    public Task<Recipe> CreateRecipe(Update update);
    public Task<Recipe?> GetRecipe(long? id);
    public Task AddServingsNumber(string? message, long? id);
    public Task AddAllCalories(long? id);
    public Task AddPFC(long? id);
    public Task AddReadyInMinutes(string? message, long? id);
    public Task AddWeightPerServing(long? id);
    public Task AddProduct(Product? product, long? id);
    public Task AddProducts(ICollection<Product>? products, long? id);
    public Task AddDescription(string? message, long? id);
    public Task CreateRecipeFromResponse(ResponseRecipe? response, long id);
    public Task<ICollection<Recipe>?> GetAllRecipes(long? id);
    public Task DeleteRecipe(long? id);
}