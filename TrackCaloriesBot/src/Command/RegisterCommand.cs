using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Command;

public class RegisterCommand : ICommand
{
    private readonly IUserRepo _userRepo;
    public string Key => Commands.RegisterCommand;
    
    public RegisterCommand(IUserRepo userRepo)
    {
        _userRepo = userRepo;
    }
    
    public async Task Execute(Update? update, ITelegramBotClient client)
    {
        var message = update.Type is UpdateType.CallbackQuery ? update.CallbackQuery?.Message! : update.Message!;
        var userData = await _userRepo.GetUser(message.Chat.Id)!;
        
        if (userData is null)
        {
            await _userRepo.CreateUser(update);

            await client.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Write your name");
        }

        switch (userData.RegistrationStage)
        {
            case 1:
                await _userRepo.AddName(update);
                await _userRepo.IncrementStage(update);

                await client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Write your age");
                break;
            case 2:
                await _userRepo.AddAge(update);
                if (userData.Age < 12) 
                {
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "You are too young!\nTry again");
                    break;
                }
                if (userData.Age > 90)
                {
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "You are too old!\nTry again");
                    break;
                }
                await _userRepo.IncrementStage(update);
                    
                await client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Write your current weight in kg \n (example: 65.10)");
                break;
            case 3:
                await _userRepo.AddWeight(update);
                if (userData.Weight is < 30 or > 200)
                {
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "I don't believe in it, write your actual weight");
                    break;
                }
                await _userRepo.IncrementStage(update);  
                
                await client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Write your height in cm");
                break;
            case 4:
                await _userRepo.AddHeight(update);
                if (userData.Height is < 60 or > 250)
                {
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "I don't believe in it, write your actual height");
                    break;
                }
                await _userRepo.IncrementStage(update);
                    
                await client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Choose your gender",
                    replyMarkup: KeyboardMarkups.GenderKeyboardMarkup);
                break;
            case 5:
                await _userRepo.AddGender(update);
                await _userRepo.IncrementStage(update);
                    
                await client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Choose your goal",
                    replyMarkup: KeyboardMarkups.GoalKeyboardMarkup);
                break;
            case 6:
                await _userRepo.AddGoal(update);
                await _userRepo.IncrementStage(update);
                    
                await client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Write your goal weight",
                    replyMarkup: new ReplyKeyboardRemove());
                break;
            case 7:
                await _userRepo.AddGoalWeight(update);
                
                if (userData.Weight is < 30 or > 200)
                {
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "It is not a healthy weight, try again");
                    break;
                }
                await _userRepo.IncrementStage(update);
                    
                await client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Write your projected progress in kg for week \n (example: 0.5)");
                break;
            case 8:
                await _userRepo.AddProjectedProgress(update);
                if (userData.ProjectedProgress is < 0 or >= 1)
                {
                    await client.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Inappropriate output. A number should be between 0 and 1 kg.\n" +
                              "Try one more time");
                    break;
                }
                await _userRepo.IncrementStage(update);
                    
                await client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Choose your activity level",
                    replyMarkup: KeyboardMarkups.ActivityKeyboardMarkup);
                break;
            case 9:
                await _userRepo.AddActivityLevel(update);
                await _userRepo.IncrementStage(update);
                    
                await client.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "You are successfully registered!",
                    replyMarkup: KeyboardMarkups.MenuKeyboardMarkup);
                break;
        }
    }
}