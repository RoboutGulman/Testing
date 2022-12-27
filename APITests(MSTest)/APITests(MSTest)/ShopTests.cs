using System.Collections.Generic;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Shop.Services.Shop;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Schema;

namespace ApiTests;

internal static class CustomProductAssert
{
    public static void IsSameProduct(this Assert assert, JToken expected, JToken actual)
    {
        Assert.AreEqual(expected["title"], actual["title"], "Titles of products not equal");
        Assert.AreEqual(expected["alias"], actual["alias"], "Aliases of products not equal");
        Assert.AreEqual(expected["price"], actual["price"], "Prices of products not equal");
        Assert.AreEqual(expected["old_price"], actual["old_price"], "Old Prices  of products not equal");
        Assert.AreEqual(expected["status"], actual["status"], "Statuses  of products not equal");
        Assert.AreEqual(expected["keyword"], actual["keyword"], "Keywords  of products not equal");
        Assert.AreEqual(expected["description"], actual["description"], "Descriptions  of products not equal");
        Assert.AreEqual(expected["hit"], actual["hit"], "Hit  of products not equal");
    }
}

[TestClass]
public class ShopApiTests
{
    private static readonly HttpClient _httpClient = new();
    private static readonly ShopAPI _api = new(_httpClient);

    private static readonly List<int> _testProductsIds = new();

    private static readonly string _productSchemaJson = File.ReadAllText(@"..\..\..\JsonSchemas\productSchema.json");
    private static readonly string _productsListSchemaJson =
        File.ReadAllText(@"..\..\..\JsonSchemas\productsListSchema.json");
    private static readonly string _addProductResponseSchemaJson =
        File.ReadAllText(@"..\..\..\JsonSchemas\addProductResponseSchema.json");
    private static readonly JObject _addProductTestsJson =
        JObject.Parse(File.ReadAllText(@"..\..\..\JsonTestCases\addProductTests.json"));
    private static readonly JObject _editProductTestsJson =
        JObject.Parse(File.ReadAllText(@"..\..\..\JsonTestCases\editProductTests.json"));

    private static bool IsJsonValid(string schemaString, JToken json)
    {
        JSchema schema = JSchema.Parse(schemaString);

        if (!json.IsValid(schema))
        {
            return false;
        }

        return true;
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        foreach (var id in _testProductsIds)
        {
            await _api.DeleteProduct(id);
        }
        _testProductsIds.Clear();
    }

    [TestMethod]
    public async Task Get_Products_And_Validate_With_Json_Schema()
    {
        var products = await _api.GetProducts();

        Assert.IsTrue(products.Count > 0, "Array of products is empty");
        Assert.IsTrue(IsJsonValid(_productsListSchemaJson, products), "Json response not valid");
    }

    // FIXED: Добавлены проверки на статусы
    [TestMethod]
    public async Task Add_Valid_Product()
    {
        var expectedStatus = 1;

        var product = _addProductTestsJson["valid"]!;

        var response = await _api.AddProduct(product.ToObject<Product>()!);

        var productId = response["id"]!.ToObject<int>();
        _testProductsIds.Add(productId);

        var actualProduct = Product.GetProductById(productId, await _api.GetProducts());

        Assert.AreEqual(expectedStatus, response["status"]);
        Assert.IsTrue(IsJsonValid(_addProductResponseSchemaJson, response));
        Assert.IsNotNull(actualProduct, "Product not found");
        Assert.IsTrue(IsJsonValid(_productSchemaJson, actualProduct), "Json response not valid");
        Assert.That.IsSameProduct(product, actualProduct);
    }

    // FIXED: Тест параметризирован
    [DataRow("invalidByCategoryIdLess")]
    [DataRow("invalidByCategoryIdMore")]
    [TestMethod]
    public async Task Add_Product_With_Wrong_Category_Id(string testCaseName)
    {
        var expectedStatus = 1;

        var result = await _api.AddProduct(_addProductTestsJson[testCaseName]!.ToObject<Product>()!);

        var productId = result["id"]!.ToObject<int>();
        Assert.AreEqual(expectedStatus, result["status"]);
        _testProductsIds.Add(productId);
        Assert.IsNull(Product.GetProductById(productId, await _api.GetProducts()),
                      "Product with wrong group id was added");
    }

    // FIXED:проверки на алиас пересены в общую функцию
    [TestMethod]
    public async Task Add_Products_With_Same_Title()
    {
        var expectedStatus = 1;
        List<string> expectedAliases = new() { "chasy", "chasy-0", "chasy-0-0" };
        var expectedProduct = _addProductTestsJson["validWithWatchTitle"]!;

        foreach (var alias in expectedAliases)
        {
            var response = await _api.AddProduct(expectedProduct.ToObject<Product>()!);
            Assert.AreEqual(expectedStatus, response["status"]);

            var productId = response["id"]!.ToObject<int>();
            var actualProduct = Product.GetProductById(productId, await _api.GetProducts())!;

            _testProductsIds.Add(productId);

            expectedProduct["alias"] = alias;
            Assert.That.IsSameProduct(expectedProduct, actualProduct);
        }
    }

    [TestMethod]
    public async Task Edit_Existing_Product()
    {
        var expectedStatus = 1;

        var response = await _api.AddProduct(_addProductTestsJson["validProductForEditing"]!.ToObject<Product>()!);
        var productId = response["id"]!.ToObject<int>();
        _testProductsIds.Add(productId);
        var editProduct = _editProductTestsJson["valid"]!;
        editProduct["id"] = productId;

        await _api.EditProduct(editProduct.ToObject<Product>()!);

        var actualProduct = Product.GetProductById(productId, await _api.GetProducts())!;

        Assert.AreEqual(expectedStatus, response["status"]!, "Expected to receive successfull status code after edit");
        Assert.That.IsSameProduct(editProduct, actualProduct);
    }

    
    [TestMethod]
    public async Task Edit_Not_Existing_Product()
    {
        var expectedStatus = 1;
        var expectedProduct = _editProductTestsJson["validWithNotExistingId"]!;

        var response = await _api.EditProduct(expectedProduct.ToObject<Product>()!);

        var editedProduct = (await _api.GetProducts()).Last();
        _testProductsIds.Add(editedProduct["id"]!.ToObject<int>());

        Assert.That.IsSameProduct(expectedProduct, editedProduct);
        Assert.AreEqual(expectedStatus, response["status"]!, "Expected to receive successfull status code after edit");
    }

    [TestMethod]
    public async Task Edit_Product_Without_Id()
    {
        var expectedStatus = 0;

        var response = await _api.EditProduct(_editProductTestsJson["validWithoutId"]!.ToObject<Product>()!);

        Assert.AreEqual(expectedStatus, response["status"]!.ToObject<int>(),
                        "Expected to get unsuccessfull status after trying editing product without specified id");
    }

    [TestMethod]
    public async Task Delete_Product()
    {
        var expectedStatus = 1;
        var response = await _api.AddProduct(_addProductTestsJson["validProductForEditing"]!.ToObject<Product>()!);
        var productId = response["id"]!.ToObject<int>();
        // FIXED: удалено добавление id в cleanup массив

        response = await _api.DeleteProduct(productId);

        Assert.AreEqual(expectedStatus, response["status"]!.ToObject<int>(),
                        "Expected to get 0 status after trying editing product without specified id");
        Assert.IsNull(Product.GetProductById(productId, await _api.GetProducts()),
                      "Expected to not find recently deleted product in product list");
    }

    [TestMethod]
    public async Task Delete_Not_Existing_Product()
    {
        var expectedStatus = 0;
        var productId = 56789;

        var result = await _api.DeleteProduct(productId);

        Assert.AreEqual(expectedStatus, result["status"]!.ToObject<int>(),
                        "Expected to get 0 status after trying deleting not existing product");
        Assert.IsNull(Product.GetProductById(productId, await _api.GetProducts()),
                      "Expected to not find non-existent in product list");
    }
}
