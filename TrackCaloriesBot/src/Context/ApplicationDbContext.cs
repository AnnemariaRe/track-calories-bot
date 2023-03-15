using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
using TrackCaloriesBot.Entity;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace TrackCaloriesBot.Context;

public class ApplicationDbContext : DbContext
{
    public Microsoft.EntityFrameworkCore.DbSet<User> Users { get; set; }
    public Microsoft.EntityFrameworkCore.DbSet<Product> Products { get; set; }
    public Microsoft.EntityFrameworkCore.DbSet<Recipe?> Recipes { get; set; }
    public Microsoft.EntityFrameworkCore.DbSet<MealData> MealData { get; set; }
    public Microsoft.EntityFrameworkCore.DbSet<DayTotalData> DayTotalData { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public ApplicationDbContext()
    {
    }
}