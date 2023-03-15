using Telegram.Bot.Types;
using TrackCaloriesBot.Entity;

namespace TrackCaloriesBot.Repository.Interfaces;

public interface IRecipeRepo
{
    public Task<Recipe?> CreateRecipe(Update update);
    public Task<Recipe?> GetRecipe(int? id);
    public Task AddServingsNumber(string? message, int? id);
    public Task AddAllCalories(int? id);
    public Task AddPFC(int? id);
    public Task AddReadyInMinutes(string? message, int? id);
    public Task AddWeightPerServing(int? id);
    public Task AddProduct(Product? product, int? id);
    public Task AddProducts(ICollection<Product>? products, int? id);
    public Task AddDescription(string? message, int? id);
    public Task CreateRecipeFromResponse(ResponseRecipe? response, long id);
    public Task<ICollection<Recipe>?> GetAllRecipes(long? id);
    public Task DeleteRecipe(int? id);
}