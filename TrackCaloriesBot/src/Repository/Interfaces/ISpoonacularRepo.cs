using TrackCaloriesBot.Entity;

namespace TrackCaloriesBot.Repository.Interfaces;

public interface ISpoonacularRepo
{
    public Task<IEnumerable<ResponseProduct>> GetProducts(string query);
    public Task<ResponseProduct?> GetProductInfo(int id);
}