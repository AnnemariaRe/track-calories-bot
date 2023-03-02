using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TrackCaloriesBot.Context;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Exceptions;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Repository;

public class RecipeRepo : IRecipeRepo
{
    private readonly ApplicationDbContext _context;

    public RecipeRepo(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Recipe> CreateRecipe(string name, int apiId = -1)
    {
        var recipe = await _context.Recipes.FirstOrDefaultAsync(
            x => x.Name == name && x.ApiId == apiId);
        
        if (recipe != null) return recipe;
        
        var newRecipe = new Recipe()
        {
            Id = new Guid().GetHashCode(),
            Name = name,
            Image = null,
            SourceUrl = null,
            ServingsNumber = 0,
            ReadyInMinutes = 0,
            WeightPerServing = 0,
            BaseProtein = 0,
            BaseFat = 0,
            BaseCarbs = 0,
        };
        
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

    public async Task DeleteRecipe(long? id)
    {
        var recipe = await GetRecipe(id)!;
        _context.Recipes.Remove(recipe!);
        await _context.SaveChangesAsync();
    }
}