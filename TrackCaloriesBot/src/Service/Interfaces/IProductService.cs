using Telegram.Bot.Types;
using TrackCaloriesBot.Entity;

namespace TrackCaloriesBot.Service.Interfaces;

public interface IProductService
{
    public Task<Product> CreateProduct(Update update);
    public Task<Product?>? GetProduct(long id);
    public Task AddName(Update update, long id);
    public Task AddServingUnit(Update update, long id);
    public Task AddServingAmount(Update update, long id);
    public Task AddCalorieAmount(Update update, long id);
    public Task AddProtein(Update update, long id);
    public Task AddFat(Update update, long id);
    public Task AddCarbs(Update update, long id);
    public Task AddQuantity(Update update, long id);
}