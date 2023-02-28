using Newtonsoft.Json;

namespace TrackCaloriesBot.Entity;

[Serializable]
public class ResponseRecipe
{
    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("image")]
    public string Image { get; set; }
    [JsonProperty("sourceUrl")]
    public string SourceUrl { get; set; }
    [JsonProperty("readyInMinutes")]
    public int ReadyInMinutes { get; set; }
    [JsonProperty("servings")]
    public int Servings { get; set; }
    [JsonProperty("weightPerServing")]
    public int WeightPerServing { get; set; }
    [JsonProperty("extendedIngredients")] 
    public ICollection<ResponseItem> Ingredients { get; set; }
    [JsonProperty("nutrition")]
    public ResponseNutrition Nutrition { get; set; }
    
}