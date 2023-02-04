using Telegram.Bot.Types;
using TrackCaloriesBot.Entity;

namespace TrackCaloriesBot.Service;

public interface IProductService
{
    public Task<Product> CreateProduct(Update update);
}