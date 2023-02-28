using TrackCaloriesBot.Entity;

namespace TrackCaloriesBot.Repository.Interfaces;

public interface ISpoonacularRepo
{
    public Task<IEnumerable<ResponseItem>> GetProducts(string query);
    public Task<ResponseItem?> GetProductInfo(int id);
}