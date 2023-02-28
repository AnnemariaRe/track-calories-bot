using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace TrackCaloriesBot.Entity;

[Serializable]
public class ResponseItem
{
    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonProperty("name")]
    public string Title { get; set; }
    [JsonProperty("image")]
    public string Image { get; set; }
    [JsonProperty("nutrition")]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ResponseNutrition Nutrition { get; set; }
    [JsonProperty("original")]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string OriginalName { get; set; }
}