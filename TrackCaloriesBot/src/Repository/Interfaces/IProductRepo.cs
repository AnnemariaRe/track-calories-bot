using Telegram.Bot.Types;
using TrackCaloriesBot.Entity;

namespace TrackCaloriesBot.Repository.Interfaces;

public interface IProductRepo
{
    public Task<Product> CreateProduct(Update update);
    public Task<Product?> GetProduct(long? id);
    public Task AddServingUnit(string? message, long? id);
    public Task AddName(string message, long? id);
    public Task AddServingAmount(string message, long? id);
    public Task AddCalorieAmount(string? message, long? id);
    public Task AddProtein(string? message, long? id);
    public Task AddFat(string? message, long? id);
    public Task AddCarbs(string? message, long? id);
    public Task AddQuantity(string message, long? id);
    public Task AddProductInfoFromResponse(ResponseItem responseProduct, long? id);
    public Task DeleteProduct(long? id);
}