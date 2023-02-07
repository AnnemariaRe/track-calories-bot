using Telegram.Bot.Types;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Service.Interfaces;

namespace TrackCaloriesBot.Service;

public class ProductService : IProductService
{
    public Task<Product> CreateProduct(Update update)
    {
        throw new NotImplementedException();
    }
}