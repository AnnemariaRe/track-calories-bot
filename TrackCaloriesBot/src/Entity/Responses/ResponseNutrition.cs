using Newtonsoft.Json;

namespace TrackCaloriesBot.Entity;

[Serializable]
public class ResponseNutrition
{
    [JsonProperty("nutrients")]
    public List<ResponseNutrient> Nutrients { get; set; }
}