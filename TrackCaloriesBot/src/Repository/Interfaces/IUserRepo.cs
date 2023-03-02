using Telegram.Bot.Types;
using TrackCaloriesBot.Entity;
using User = TrackCaloriesBot.Entity.User;

namespace TrackCaloriesBot.Repository.Interfaces;

public interface IUserRepo
{
    public Task<User> CreateUser(Update update);
    public Task<User?> GetUser(long id);
    public Task IncrementStage(Update update);
    public Task AddName(Update update);
    public Task AddAge(Update update);
    public Task AddWeight(Update update);
    public Task AddHeight(Update update);
    public Task AddGender(Update update);
    public Task AddGoal(Update update);
    public Task AddGoalWeight(Update update);
    public Task AddActivityLevel(Update update);
    public Task AddDayTotalData(DayTotalData? dayTotalData, Update update);
    public Task AddRecipe(long id, Recipe? recipe);


}