using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TrackCaloriesBot.Constant;
using TrackCaloriesBot.Entity;
using TrackCaloriesBot.Entity.Requests;
using TrackCaloriesBot.Repository.Interfaces;

namespace TrackCaloriesBot.Repository;

public class SpoonacularRepo : ISpoonacularRepo
{
    public async Task<IEnumerable<ResponseItem>> GetProducts(string query)
    {
        var products = new List<ResponseItem>();
        
        var url = "https://api.spoonacular.com/food/ingredients/search";
        var parameters = $"?apiKey={Keys.SpoonacularApiKey}&query={query}&number=15";

        var client = new HttpClient();
        client.BaseAddress = new Uri(url);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync(parameters).ConfigureAwait(false);  
       
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var productList = JsonConvert.DeserializeObject<ResponseItemList>(jsonString);

            if (productList != null)
            {
                products.AddRange(productList.Items);
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

    public async Task<ResponseItem?> GetProductInfo(int id)
    {
        var url = "https://api.spoonacular.com/food/ingredients/";
        var parameters = $"{id}/information?apiKey={Keys.SpoonacularApiKey}&amount=100&unit=g";
        
        var client = new HttpClient();
        client.BaseAddress = new Uri(url);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync(parameters).ConfigureAwait(false);

        var product = new ResponseItem();
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            product = JsonConvert.DeserializeObject<ResponseItem>(jsonString);
        }

        return product;
    }

    public async Task<IEnumerable<ResponseItem>> GetRecipes(RequestRecipe? request, string query)
    {
        var recipes = new List<ResponseItem>();
        
        var url = "https://api.spoonacular.com/recipes/complexSearch?";
        var parameters =
            $"?apiKey={Keys.SpoonacularApiKey}&number=10&query={query}";

        if (request?.Equipments is not null)
        {
            parameters += $"&equipment={request.Equipments}";
        }
        if (request?.Ingredients is not null)
        {
            parameters += $"&includeIngredients={request.Ingredients}";
        }
        if (request?.MaxReadyTime is not null)
        {
            parameters += $"&maxReadyTime={request.MaxReadyTime}";
        }
        
        var client = new HttpClient();
        client.BaseAddress = new Uri(url);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync(parameters).ConfigureAwait(false);  
       
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var recipeList = JsonConvert.DeserializeObject<ResponseItemList>(jsonString);

            if (recipeList != null) recipes.AddRange(recipeList.Items);
        }
        
        return recipes;
    }

    public async Task<ResponseRecipe?> GetRecipeInfo(int id)
    {
        var url = "https://api.spoonacular.com/recipes/";
        var parameters = $"{id}/information?apiKey={Keys.SpoonacularApiKey}&includeNutrition=true";
        
        var client = new HttpClient();
        client.BaseAddress = new Uri(url);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var response = await client.GetAsync(parameters).ConfigureAwait(false);

        var recipe = new ResponseRecipe();
        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            recipe = JsonConvert.DeserializeObject<ResponseRecipe>(jsonString);
            var data = JsonConvert.DeserializeObject<JObject>(jsonString);
            recipe.WeightPerServing = data.SelectToken(
                "nutrition.weightPerServing.amount").Value<int>();

            if (!recipe.SourceUrl.Contains("https"))
            {
                recipe.SourceUrl = recipe.SourceUrl.Replace("http", "https");
            }
        }

        return recipe;
    }
}