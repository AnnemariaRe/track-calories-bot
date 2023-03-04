using System.ComponentModel.DataAnnotations;

namespace TrackCaloriesBot.Entity;

public class Recipe
{
    public Recipe()
    {
    }

    [Key] public int Id { get; set; }
    public int? ApiId { get; set; }
    public string? Name { get; set; }
    public string? Image { get; set; }
    public string? SourceUrl { get; set; }
    public int ServingsNumber { get; set; }
    public int ReadyInMinutes { get; set; }
    public double BaseCalories { get; set; }
    public double BaseProtein { get; set; }
    public double BaseFat { get; set; }
    public double BaseCarbs { get; set; }
    public int WeightPerServing { get; set; }
    public ICollection<Product>? Products { get; set; }
    public string? Description { get; set; }
}