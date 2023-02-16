namespace TrackCaloriesBot.Exceptions;

public class NullBotException : BotException
{
    public NullBotException(string message) : base(message)
    {
    }
}