using System.ComponentModel.DataAnnotations;

namespace TrackCaloriesBot.Entity;

[Serializable]
public class LastCommand
{
    public LastCommand(string userId, string commandKey)
    {
        UserId = userId;
        CommandKey = commandKey;
    }

    [Required]
    public string UserId { get; set; }

    public string CommandKey { get; set; }
}