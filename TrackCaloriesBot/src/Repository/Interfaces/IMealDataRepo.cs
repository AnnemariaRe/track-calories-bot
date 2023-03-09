using Telegram.Bot.Types;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Enums;

namespace TrackCaloriesBot.Repository.Interfaces;

public interface IMealDataRepo
{
    public Task<MealData> AddNewMealData(Update update);
    public Task<MealData?> GetMealData(Update update, MealType? mealType);
    public Task AddNewProduct(Product? product, Update update);
    public List<Product> GetProducts(MealData mealData);
}