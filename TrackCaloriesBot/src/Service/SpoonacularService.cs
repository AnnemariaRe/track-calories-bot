using System.Net.Http.Headers;
using Newtonsoft.Json;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Service.Interfaces;

namespace TrackCaloriesBot.Service;

public class SpoonacularService : ISpoonacularService
{
    public async Task<IEnumerable<ResponseProduct>> GetProducts(string query)
    {
        var products = new List<ResponseProduct>();
        
        var url = "https://spoonacular-recipe-food-nutrition-v1.p.rapidapi.com/food/products/search";
        var parameters = $"?query={query}&apiKey={Keys.SPOONACULAR_API_KEY}&number=7";

        var client = new HttpClient();
        client.BaseAddress = new Uri(url);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync(parameters).ConfigureAwait(false);  
       
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var productList = JsonConvert.DeserializeObject<ResponseProductList>(jsonString);

            if (productList != null)
            {
                products.AddRange(productList.Products);
            }
        }
        
        return products;
    }
}