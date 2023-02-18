using TrackCaloriesBot.Command;
using TrackCaloriesBot.Entity;

namespace TrackCaloriesBot.Repository.Interfaces;

public interface ICommandRepo
{
    public void AddCommand(LastCommand newCommand);
    public LastCommand? GetLastCommand(string userId);
}