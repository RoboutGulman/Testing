
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Web;

namespace Shop.Services.Shop;

public class ShopAPI
{
    private readonly HttpClient _httpClient;

    private struct ApiMethodsUri
    {
        public static string GetProducts = "http://shop.qatl.ru/api/products";
        public static string DeleteProduct = "http://shop.qatl.ru/api/deleteproduct";
        public static string AddProduct = "http://shop.qatl.ru/api/addproduct";
        public static string EditProduct = "http://shop.qatl.ru/api/editproduct";

        public ApiMethodsUri()
        {
        }
    }

    public ShopAPI(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<JArray> GetProducts()
    {
        var requestUri = new Uri(ApiMethodsUri.GetProducts);
        var response = await _httpClient.GetAsync(requestUri);

        return (JArray)await GetJsonFromResponse(response);
    }

    public async Task<JObject> DeleteProduct(int id)
    {
        var requestUri = new Uri(ApiMethodsUri.DeleteProduct);

        // add parametr to uri
        var uriBuilder = new UriBuilder(requestUri);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query[nameof(Product.id)] = id.ToString();
        uriBuilder.Query = query.ToString();

        requestUri = uriBuilder.Uri;

        var response = await _httpClient.GetAsync(requestUri);

        return (JObject)await GetJsonFromResponse(response);
    }

    public async Task<JObject> AddProduct(Product product)
    {
        var requestUri = new Uri(ApiMethodsUri.AddProduct);
        var response = await HttpPostProduct(requestUri, product);

        return (JObject)await GetJsonFromResponse(response);
    }

    public async Task<JObject> EditProduct(Product product)
    {
        var requestUri = new Uri(ApiMethodsUri.EditProduct);
        var response = await HttpPostProduct(requestUri, product);

        return (JObject)await GetJsonFromResponse(response);
    }

    private async Task<HttpResponseMessage> HttpPostProduct(Uri requestUri, Product product)
    {
        var json = JsonConvert.SerializeObject(product);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        return await _httpClient.PostAsync(requestUri, data);
    }

    private async Task<JToken> GetJsonFromResponse(HttpResponseMessage response)
    {
        var jsonContent = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

        var jsonContentString = jsonContent!.ToString();
        if (jsonContentString is null)
        {
            throw new Exception("Error was found while handling the response");
        }

        var result = JToken.Parse(jsonContentString);

        return result;
    }
}
