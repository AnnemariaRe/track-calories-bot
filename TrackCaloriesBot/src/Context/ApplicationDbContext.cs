using Microsoft.EntityFrameworkCore;
using TrackCaloriesBot.Entity;

namespace TrackCaloriesBot.Context;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<MealData> MealData { get; set; }
    public DbSet<DayTotalData> DayTotalData { get; set; }
    public DbSet<ConversationData> ProductConversations { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public ApplicationDbContext()
    {
    }
}