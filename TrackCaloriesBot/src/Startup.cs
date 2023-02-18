using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using TrackCaloriesBot.Command;
using TrackCaloriesBot.Context;
using TrackCaloriesBot.Repository;
using TrackCaloriesBot.Repository.Interfaces;
using TrackCaloriesBot.Service;
using TrackCaloriesBot.Service.Interfaces;

namespace TrackCaloriesBot;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private readonly IConfiguration _configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        
        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(
            _configuration.GetConnectionString("Db")));
        
        services.AddSingleton<IConnectionMultiplexer>(options =>
            ConnectionMultiplexer.Connect(_configuration.GetConnectionString("Redis")));

        services.AddSingleton<Bot>();
        services.AddSingleton<ICommandService, CommandService>();
        services.AddSingleton<IUserRepo, UserRepo>();
        services.AddSingleton<IDayTotalDataRepo, DayTotalDataRepo>();
        services.AddSingleton<IMealDataRepo, MealDataRepo>();
        services.AddSingleton<IProductRepo, ProductRepo>();
        services.AddSingleton<IConversationDataRepo, ConversationDataRepo>();
        services.AddSingleton<ISpoonacularRepo, SpoonacularRepo>();
        services.AddSingleton<ICommand, StartCommand>();
        services.AddSingleton<ICommand, RegisterCommand>();
        services.AddSingleton<ICommand, ShowUserInfoCommand>();
        services.AddSingleton<ICommand, SummaryCommand>();
        services.AddSingleton<ICommand, NewRecordCommand>();
        services.AddSingleton<ICommand, AddProductToMealCommand>();
        services.AddSingleton<ICommand, AddWaterCommand>();
        services.AddSingleton<ICommand, BackCommand>();
        services.AddSingleton<ICommand, EnterManuallyCommand>();
        services.AddSingleton<ICommand, SearchProductCommand>();
        services.AddSingleton<ICommand, SearchInlineQueryCommand>();
        
        var culture = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        serviceProvider.GetRequiredService<Bot>().GetClient().Wait();
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}