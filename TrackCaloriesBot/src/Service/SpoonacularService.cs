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
        
        var url = "https://api.spoonacular.com/food/ingredients/search";
        var parameters = $"?apiKey={Keys.SPOONACULAR_API_KEY}&query={query}&number=15";

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
                string str;
                foreach (var product in products)
                {
                    str = product.Image;
                    product.Image = "https://spoonacular.com/cdn/ingredients_100x100/" + str;
                }
            }
        }
        
        return products;
    }

    public async Task<ResponseProduct?> GetProductInfo(int id)
    {
        var url = "https://api.spoonacular.com/food/ingredients/";
        var parameters = $"{id}/information?apiKey={Keys.SPOONACULAR_API_KEY}&amount=100&unit=g";
        
        var client = new HttpClient();
        client.BaseAddress = new Uri(url);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync(parameters).ConfigureAwait(false);

        var product = new ResponseProduct();
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            product = JsonConvert.DeserializeObject<ResponseProduct>(jsonString);
        }

        return product;
    }
}