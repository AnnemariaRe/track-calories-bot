using TrackCaloriesBot.Entity;

namespace TrackCaloriesBot.Service.Interfaces;

public interface ISpoonacularService
{
    public Task<IEnumerable<ResponseProduct>> GetProducts(string query);
}