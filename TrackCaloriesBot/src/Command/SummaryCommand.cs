using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Enums;
using TrackCaloriesBot.Repository.Interfaces;
using User = TrackCaloriesBot.Entity.User;

namespace TrackCaloriesBot.Command;

public class SummaryCommand : ICommand
{
    public string Key => Commands.SummaryCommand;
    private readonly IUserRepo _userRepo;
    private readonly IDayTotalDataRepo _dayTotalDataRepo;
    private readonly IMealDataRepo _mealDataRepo;
    
    public SummaryCommand(IUserRepo userRepo, IDayTotalDataRepo dayTotalDataRepo, IMealDataRepo mealDataRepo)
    {
        _userRepo = userRepo;
        _dayTotalDataRepo = dayTotalDataRepo;
        _mealDataRepo = mealDataRepo;
    }

    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        var userData = await _userRepo.GetUser(update.Message.Chat.Id)!;

        if (userData is null)
        {
            await client.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: "Welcome to the Track Calories Bot!",
                replyMarkup: InlineKeyboards.StartInlineKeyboard);
        }
        else
        {
            await client.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: BeautifulOutput(userData, update),
                parseMode: ParseMode.Html);
        }
    }
    
    private string BeautifulOutput(User userData, Update update)
    {
        var dayTotal = _dayTotalDataRepo.GetDayTotalData(update)?.Result;

        if (dayTotal is null) return "No information for today :(";
        
        var caloriesNeed = HarrisBenedictFormula(userData);
        var proteinNeed = Math.Round(0.25 * caloriesNeed / 4);
        var fatNeed = Math.Round(0.3 * caloriesNeed / 9);
        var carbsNeed = Math.Round(0.45 * caloriesNeed / 4);
        
        var breakfastData = _dayTotalDataRepo.GetMealData(MealType.Breakfast, dayTotal);
        var lunchData = _dayTotalDataRepo.GetMealData(MealType.Lunch, dayTotal);
        var dinnerData = _dayTotalDataRepo.GetMealData(MealType.Dinner, dayTotal);
        var snackData = _dayTotalDataRepo.GetMealData(MealType.Snack, dayTotal);
        
        var mealDataCalories = new double[4];
        var mealDataProtein = new double[4];
        var mealDataFat = new double[4];
        var mealDataCarbs = new double[4];
        
        if (breakfastData is { Result: { } })
        {
            mealDataCalories[0] += _mealDataRepo.GetProducts(breakfastData.Result).Sum(product =>
                product.BaseCalories * (product.ServingAmount / 100.0) * product.Quantity);
            mealDataProtein[0] += _mealDataRepo.GetProducts(breakfastData.Result).Sum(product =>
                    product.BaseProtein * (product.ServingAmount / 100.0) * product.Quantity);
            mealDataFat[0] += _mealDataRepo.GetProducts(breakfastData.Result).Sum(product =>
                product.BaseFat * (product.ServingAmount / 100.0) * product.Quantity);
            mealDataCarbs[0] += _mealDataRepo.GetProducts(breakfastData.Result).Sum(product =>
                product.BaseCarbs * (product.ServingAmount / 100.0) * product.Quantity);
        }

        if (lunchData is { Result: { } })
        {
            mealDataCalories[1] += _mealDataRepo.GetProducts(lunchData.Result).Sum(product =>
                product.BaseCalories * (product.ServingAmount / 100.0) * product.Quantity);
            mealDataProtein[1] += _mealDataRepo.GetProducts(lunchData.Result).Sum(product =>
                product.BaseProtein * (product.ServingAmount / 100.0) * product.Quantity);
            mealDataFat[1] += _mealDataRepo.GetProducts(lunchData.Result).Sum(product =>
                product.BaseFat * (product.ServingAmount / 100.0) * product.Quantity);
            mealDataCarbs[1] += _mealDataRepo.GetProducts(lunchData.Result).Sum(product =>
                product.BaseCarbs * (product.ServingAmount / 100.0) * product.Quantity);
        }

        if (dinnerData is { Result: { } })
        {
            mealDataCalories[2] += _mealDataRepo.GetProducts(dinnerData.Result).Sum(product =>
                product.BaseCalories * (product.ServingAmount / 100.0) * product.Quantity);
            mealDataProtein[2] += _mealDataRepo.GetProducts(dinnerData.Result).Sum(product =>
                product.BaseProtein * (product.ServingAmount / 100.0) * product.Quantity);
            mealDataFat[2] += _mealDataRepo.GetProducts(dinnerData.Result).Sum(product =>
                product.BaseFat * (product.ServingAmount / 100.0) * product.Quantity);
            mealDataCarbs[2] += _mealDataRepo.GetProducts(dinnerData.Result).Sum(product =>
                product.BaseCarbs * (product.ServingAmount / 100.0) * product.Quantity);
        }
        
        if (snackData is { Result: { } })
        {
            mealDataCalories[3] += _mealDataRepo.GetProducts(snackData.Result).Sum(product =>
                product.BaseCalories * (product.ServingAmount / 100.0) * product.Quantity);
            mealDataProtein[3] += _mealDataRepo.GetProducts(snackData.Result).Sum(product =>
                product.BaseProtein * (product.ServingAmount / 100.0) * product.Quantity);
            mealDataFat[3] += _mealDataRepo.GetProducts(snackData.Result).Sum(product =>
                product.BaseFat * (product.ServingAmount / 100.0) * product.Quantity);
            mealDataCarbs[3] += _mealDataRepo.GetProducts(snackData.Result).Sum(product =>
                product.BaseCarbs * (product.ServingAmount / 100.0) * product.Quantity);
        }
        
        var caloriesAte = mealDataCalories.Sum();
        var proteinAte = mealDataProtein.Sum();
        var fatAte = mealDataFat.Sum();
        var carbsAte = mealDataCarbs.Sum();
        var waterDrank = dayTotal.Water;

        return @"--------------- <b>EATEN \ LEFT</b> -----------------" + "\n" + "<pre>Calories:  " +
               @$"{caloriesAte:0} \ {Math.Max(0, caloriesNeed - caloriesAte):0} kcal" + "\n" + "Protein:   " +
               @$"{proteinAte:0} \ {Math.Max(0, proteinNeed - proteinAte):0} g" + "\n" + "Fat:       " +
               @$"{fatAte:0} \ {Math.Max(0, fatNeed - fatAte):0} g" + "\n" + "Carbs:     " +
               @$"{carbsAte:0} \ {Math.Max(0, carbsNeed - carbsAte):0} g" + "</pre>\n\n" +
               "------------- <b>EATEN PER MEAL</b> -------------" + "\n" + "<pre>Breakfast: " +
               $"{mealDataCalories[0]:0} kcal" + "\n" + "Lunch:     " + $"{mealDataCalories[1]:0} kcal" + "\n" + "Dinner:    " +
               $"{mealDataCalories[2]:0} kcal" + "\n" + "Snack:     " + $"{mealDataCalories[3]:0} kcal" + "</pre>\n\n" +
               @"--------------- <b>DRANK \ LEFT</b> ----------------" + "\n" + "<pre>Water:  " +
               $@"{waterDrank:0.00} \ {(userData.Weight * 0.03 - waterDrank):0.00} L" + "</pre>";
    }

    private double HarrisBenedictFormula(User userData)
    {
        double result;
        if (userData.Gender is Gender.Female)
        {
            result = 447.593 + (9.247 * userData.Weight) + (3.098 * userData.Height) - (4.33 * userData.Age);
            switch (userData.ActivityLevel)
            {
                case ActivityLevel.Low:
                    result *= 1.2;
                    break;
                case ActivityLevel.Moderate:
                    result *= 1.375;
                    break;
                case ActivityLevel.High:
                    result *= 1.55;
                    break;
                case ActivityLevel.VeryHigh:
                    result *= 1.725;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        } 
        else
        {
            result = 88.362 + (13.397 * userData.Weight) + (4.799 * userData.Height) - (5.677 * userData.Age);
            switch (userData.ActivityLevel)
            {
                case ActivityLevel.Low:
                    result *= 1.2;
                    break;
                case ActivityLevel.Moderate:
                    result *= 1.375;
                    break;
                case ActivityLevel.High:
                    result *= 1.55;
                    break;
                case ActivityLevel.VeryHigh:
                    result *= 1.725;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        switch (userData.Goal)
        {
            case Goal.Lose:
                result *= 0.85;
                break;
            case Goal.Gain:
                result *= 1.15;
                break;
            case Goal.Maintain:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        return result;
    }
}