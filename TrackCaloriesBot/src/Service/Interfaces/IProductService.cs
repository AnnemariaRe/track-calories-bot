using Telegram.Bot.Types;
using TrackCaloriesBot.Entity;

namespace TrackCaloriesBot.Service.Interfaces;

public interface IProductService
{
    public Task<Product> CreateProduct(Update update, int productId = 0);
    public Task<Product?>? GetProduct(long? id);
    public Task AddServingUnit(string message, long? id);
    public Task AddName(string message, long? id);
    public Task AddServingAmount(string message, long? id);
    public Task AddCalorieAmount(string? message, long? id);
    public Task AddProtein(string message, long? id);
    public Task AddFat(string message, long? id);
    public Task AddCarbs(string message, long? id);
    public Task AddQuantity(string message, long? id);
    public Task AddProductInfoFromResponse(ResponseProduct responseProduct);
}