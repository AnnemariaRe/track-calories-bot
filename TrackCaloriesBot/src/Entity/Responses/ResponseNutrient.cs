using Newtonsoft.Json;

namespace TrackCaloriesBot.Entity;

[Serializable]
public class ResponseNutrient
{
    public ResponseItem Item { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("amount")]
    public double Amount { get; set; }
    [JsonProperty("unit")]
    public string Unit { get; set; }
}