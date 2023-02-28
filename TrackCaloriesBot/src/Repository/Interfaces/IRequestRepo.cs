using Telegram.Bot.Types;
using TrackCaloriesBot.Entity.Requests;

namespace TrackCaloriesBot.Repository.Interfaces;

public interface IRequestRepo
{
    public RequestRecipe? CreateRequest(Update update);
    public RequestRecipe? GetRequest(long? id);
    public void AddIngredients(long? id, string? data);
    public void AddEquipments(long? id, string? data);
    public void AddMaxReadyTime(long? id, string? data);
    public void DeleteRequest(RequestRecipe request);
}