using Newtonsoft.Json;

namespace TrackCaloriesBot.Entity;

[Serializable]
public class ResponseItemList
{
    [JsonProperty("results")]
    public IEnumerable<ResponseItem> Items { get; set; }
}