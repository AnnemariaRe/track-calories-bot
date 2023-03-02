using Telegram.Bot.Types;
using TrackCaloriesBot.Entity;

namespace TrackCaloriesBot.Repository.Interfaces;

public interface IRecipeRepo
{
    public Task<Recipe> CreateRecipe(string name, int apiId);
    public Task<Recipe?> GetRecipe(long? id);
    public Task DeleteRecipe(long? id);
}