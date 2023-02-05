using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TrackCaloriesBot.Command;
using TrackCaloriesBot.Context;
using TrackCaloriesBot.Service;

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
            ""));

        services.AddSingleton<Bot>();
        services.AddSingleton<ICommandService, CommandService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IDayTotalDataService, DayTotalDataService>();
        services.AddSingleton<IMealDataService, MealDataService>();
        services.AddSingleton<IProductService, ProductService>();
        services.AddSingleton<ICommand, StartCommand>();
        services.AddSingleton<ICommand, RegisterCommand>();
        services.AddSingleton<ICommand, ShowUserInfoCommand>();
        services.AddSingleton<ICommand, SummaryCommand>();
        services.AddSingleton<ICommand, NewRecordCommand>();
        services.AddSingleton<ICommand, AddProductToMealCommand>();
        services.AddSingleton<ICommand, AddWaterCommand>();
        services.AddSingleton<ICommand, BackCommand>();
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