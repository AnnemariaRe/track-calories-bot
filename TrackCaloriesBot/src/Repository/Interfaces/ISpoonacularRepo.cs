using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Entity.Requests;

namespace TrackCaloriesBot.Repository.Interfaces;

public interface ISpoonacularRepo
{
    public Task<IEnumerable<ResponseItem>> GetProducts(string query);
    public Task<ResponseItem?> GetProductInfo(int id);
    public Task<IEnumerable<ResponseItem>> GetRecipes(RequestRecipe request, string query);
    public Task<ResponseRecipe?> GetRecipeInfo(int id);
}