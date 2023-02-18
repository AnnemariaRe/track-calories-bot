using Newtonsoft.Json;
using StackExchange.Redis;
using TrackCaloriesBot.Command;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Repository;

public class CommandRepo : ICommandRepo
{
    private readonly IConnectionMultiplexer _redis;

    public CommandRepo(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public void AddCommand(LastCommand newCommand)
    {
        if (newCommand == null)
        {
            throw new ArgumentOutOfRangeException(nameof(newCommand));
        }

        var db = _redis.GetDatabase();
        var serialCommand = JsonConvert.SerializeObject(newCommand);

        db.StringSet(newCommand.UserId, serialCommand);
    }

    public LastCommand? GetLastCommand(string userId)
    {
        var db = _redis.GetDatabase();
        var command = db.StringGet(userId);

        return !string.IsNullOrEmpty(command) ? JsonConvert.DeserializeObject<LastCommand>(command) : null;
    }
}