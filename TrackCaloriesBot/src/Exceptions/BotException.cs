namespace TrackCaloriesBot.Exceptions;

public class BotException : Exception
{
    public BotException(string message) : base(message)
    {
    }
}