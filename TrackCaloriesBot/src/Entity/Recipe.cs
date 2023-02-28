using System.ComponentModel.DataAnnotations;

namespace TrackCaloriesBot.Entity;

public class Recipe
{
    public Recipe()
    {
    }

    [Key] public int Id { get; set; }
    public string? Name { get; set; }
    public string Image { get; set; }
    public string SourceUrl { get; set; }
    public int ServingsNumber { get; set; }
    public int ReadyInMinutes { get; set; }
    public int BaseProtein { get; set; }
    public int BaseFat { get; set; }
    public int BaseCarbs { get; set; }
    public int WeightPerServing { get; set; }
    public ICollection<Product> Products { get; set; }
}