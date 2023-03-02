using Newtonsoft.Json;
using StackExchange.Redis;
using Telegram.Bot.Types;
using TrackCaloriesBot.Entity.Requests;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Repository;

public class RequestRepo : IRequestRepo
{
    private const string RequestIdPrefix = "request";
    private readonly IConnectionMultiplexer _redis;

    public RequestRepo(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public RequestRecipe? CreateRequest(Update update)
    {
        var db = _redis.GetDatabase();

        var newRequest = new RequestRecipe()
        {
            UserId = RequestIdPrefix + update.Message.Chat.Id,
            Equipments = null,
            Ingredients = null,
            MaxReadyTime = null
        };
        
        var serialRequest = JsonConvert.SerializeObject(newRequest);
        db.StringSet(newRequest.UserId.ToString(), serialRequest);

        return newRequest;
    }

    public RequestRecipe? GetRequest(long? id)
    {
        var db = _redis.GetDatabase();
        var request = db.StringGet(RequestIdPrefix + id.ToString());

        return !string.IsNullOrEmpty(request) ? JsonConvert.DeserializeObject<RequestRecipe>(request) : null;
    }

    public void AddIngredients(long? id, string? data)
    {
        if (data is null) return;
        
        var db = _redis.GetDatabase();
        var request = GetRequest(id);
        if (request is not null)
        {
            request.Ingredients = data;
            var serialRequest = JsonConvert.SerializeObject(request);
            db.StringSet(RequestIdPrefix + id.ToString(), serialRequest);
        }
    }

    public void AddEquipments(long? id, string? data)
    {
        if (data is null) return;
        
        var db = _redis.GetDatabase();
        var request = GetRequest(id);
        if (request is not null)
        {
            request.Equipments = data;
            var serialRequest = JsonConvert.SerializeObject(request);
            db.StringSet(RequestIdPrefix + id.ToString(), serialRequest);
        }
    }

    public void AddMaxReadyTime(long? id, string? data)
    {
        if (data is null) return;
        
        var db = _redis.GetDatabase();
        var request = GetRequest(id);
        if (request is not null && int.TryParse(data, out var x))
        {
            request.MaxReadyTime = x;
            var serialRequest = JsonConvert.SerializeObject(request);
            db.StringSet(RequestIdPrefix + id.ToString(), serialRequest);
        }
    }

    public void DeleteRequest(RequestRecipe request)
    {
        var db = _redis.GetDatabase();
        db.KeyDelete(request.UserId.ToString());
    }
}