using Microsoft.EntityFrameworkCore;
using TrackCaloriesBot.Entity;

namespace TrackCaloriesBot.Context;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public ApplicationDbContext()
    {
    }
}