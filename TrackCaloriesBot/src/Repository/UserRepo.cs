using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TrackCaloriesBot.Context;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Enums;
using TrackCaloriesBot.Exceptions;
using TrackCaloriesBot.Repository.Interfaces;
using User = TrackCaloriesBot.Entity.User;

namespace TrackCaloriesBot.Repository;

public class UserRepo : IUserRepo
{
    private readonly ApplicationDbContext _context;

    public UserRepo(ApplicationDbContext context) 
    {
        _context = context;
    }

    public async Task<User> CreateUser(Update update)
    {
        var upd = update.CallbackQuery;
        var user = await _context.Users.FirstOrDefaultAsync(x => x.TgId == upd.Message.Chat.Id);
                 
        if (user != null) return user;
        
        var newUser = new User
        {
            TgId = upd.Message.Chat.Id, 
            Username = upd.From.Username,
            Name = null,
            Age = 0,
            Gender = null,
            Weight = 0,
            Height = 0,
            GoalWeight = 0,
            Goal = null,
            ActivityLevel = null,
            RegistrationStage = 1,
            Recipes = new List<Recipe>()
        };

        var result = await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();

        return result.Entity;
    }

    public async Task IncrementStage(Update update)
    {
        if (update.Message?.Text != null)
        {
            var user = GetUser(update.Message.Chat.Id);
            if (user?.Result is not null)
            {
                user.Result.RegistrationStage++;
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddName(Update update)
    {
        if (update.Message?.Text != null)
        {
            var user = GetUser(update.Message.Chat.Id);
            if (user?.Result is not null)
            {
                user.Result.Name = update.Message.Text;
                await _context.SaveChangesAsync();
            }
        }
    }
    
    public async Task AddAge(Update update)
    {
        var user = GetUser(update.Message.Chat.Id);
        if (user?.Result is not null)
        {
            var check = int.TryParse(update.Message.Text, out var x);
            if (check)
            {
                user.Result.Age = x; 
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddWeight(Update update)
    {
        var user = GetUser(update.Message.Chat.Id);
        if (user?.Result is not null)
        {
            var check = float.TryParse(update.Message.Text, out var x);
            if (check)
            {
                user.Result.Weight = x; 
                await _context.SaveChangesAsync();
            }
        }
    }
    
    public async Task AddHeight(Update update)
    {
        var user = GetUser(update.Message.Chat.Id);
        if (user?.Result is not null)
        {
            var check = float.TryParse(update.Message.Text, out var x);
            if (check)
            {
                user.Result.Height = x; 
                await _context.SaveChangesAsync();
            }
        }
    }
    
    public async Task AddGender(Update update)
    {
        var user = GetUser(update.Message.Chat.Id);
        if (user?.Result is not null)
        {
            user.Result.Gender = update.Message.Text switch
            {
                "Female" => Gender.Female,
                "Male" => Gender.Male,
            };
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task AddGoal(Update update)
    {
        var user = GetUser(update.Message.Chat.Id);
        if (user?.Result is not null)
        {
            user.Result.Goal = update.Message.Text switch
            {
                "Lose weight" => Goal.Lose,
                "Maintain weight" => Goal.Maintain,
                "Gain weight" => Goal.Gain,
            };
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddGoalWeight(Update update)
    {
        var user = GetUser(update.Message.Chat.Id);
        if (user?.Result is not null)
        {
            var check = float.TryParse(update.Message.Text, out var x);
            if (check)
            {
                user.Result.GoalWeight = x; 
                await _context.SaveChangesAsync();
            }
        }
    }

    public async Task AddActivityLevel(Update update)
    {
        var user = GetUser(update.Message.Chat.Id);
        if (user?.Result is not null)
        {
            user.Result.ActivityLevel = update.Message.Text switch
            {
                "Low" => ActivityLevel.Low,
                "Moderate" => ActivityLevel.Moderate,
                "High" => ActivityLevel.High,
                "Very high" => ActivityLevel.VeryHigh,
            };
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddDayTotalData(DayTotalData? dayTotalData, Update update)
    {
        var user = GetUser(update.Message.Chat.Id);
        if (user?.Result is not null && dayTotalData is not null)
        {
            user.Result.DayTotalData.Add(dayTotalData);
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddRecipe(long id, Recipe? recipe)
    {
        var user = GetUser(id);
        if (user?.Result is not null && recipe is not null)
        {
            try
            {
                user.Result.Recipes.Add(recipe);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            await _context.SaveChangesAsync();
        }
    }

    public async Task<User?> GetUser(long id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.TgId == id);
        return user;
    }
    
}