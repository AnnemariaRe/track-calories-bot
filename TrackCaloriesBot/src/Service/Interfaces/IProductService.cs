using Telegram.Bot.Types;
using TrackCaloriesBot.Entity;

namespace TrackCaloriesBot.Service.Interfaces;

public interface IProductService
{
    public Task<Product> CreateProduct(Update update);
}