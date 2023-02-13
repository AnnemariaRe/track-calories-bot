using Newtonsoft.Json;

namespace TrackCaloriesBot.Entity;

[Serializable]
public class ResponseProductList
{
    [JsonProperty("results")]
    public IEnumerable<ResponseProduct> Products { get; set; }
}