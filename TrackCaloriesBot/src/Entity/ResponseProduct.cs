using Newtonsoft.Json;

namespace TrackCaloriesBot.Entity;

[Serializable]
public class ResponseProduct
{
    [JsonProperty("id")]
    public long ProductId { get; set; }
    [JsonProperty("name")]
    public string Title { get; set; }
    [JsonProperty("image")]
    public string Image { get; set; }
}