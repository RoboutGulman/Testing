using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Shop.Services.Shop;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Schema;

namespace ApiTests;

internal static class CustomProductAssert
{
    public static void IsSameProduct(this Assert assert, JToken expected, JToken actual)
    {
        Assert.AreEqual(expected["title"], actual["title"], "Expected to get same title as edit json has");
        Assert.AreEqual(expected["price"], actual["price"], "Expected to get same price as edit json has");
        Assert.AreEqual(expected["old_price"], actual["old_price"], "Expected to get same old_price as edit json has");
        Assert.AreEqual(expected["status"], actual["status"], "Expected to get same status as edit json has");
        Assert.AreEqual(expected["keyword"], actual["keyword"], "Expected to get same keyword as edit json has");
        Assert.AreEqual(expected["description"], actual["description"],
                        "Expected to get same description as edit json has");
        Assert.AreEqual(expected["hit"], actual["hit"], "Expected to get same hit as edit json has");
    }
}

[TestClass]
public class ShopApiTests
{
    private static readonly Uri _baseUri = new("http://shop.qatl.ru/");
    private static readonly HttpClient _httpClient = new();
    private static readonly ShopAPI _api = new(_baseUri, _httpClient);

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
    public async Task Get_Products()
    {
        // arrange

        // act
        var response = await _api.GetProducts();

        // assert
        Assert.IsTrue(response.Count > 0, "Array of products is empty");
    }

    [TestMethod]
    public async Task Get_Products_And_Validate_With_Json_Schema()
    {
        // arrange

        // act
        var products = await _api.GetProducts();

        // assert
        Assert.IsTrue(IsJsonValid(_productsListSchemaJson, products),
                      "Expected array of products, got data that doesn't match products list schema");
    }

    [TestMethod]
    public async Task Add_Valid_Product()
    {
        // arrange
        var product = _addProductTestsJson["valid"]!;

        // act
        var response = await _api.AddProduct(product.ToObject<Product>()!);

        // assert
        var productId = response["id"]!.ToObject<int>();
        _testProductsIds.Add(productId);

        var actualProduct = Product.GetProductById(productId, await _api.GetProducts());

        Assert.IsTrue(IsJsonValid(_addProductResponseSchemaJson, response));
        Assert.IsNotNull(actualProduct, "Expected to find recently created valid product in product list");
        Assert.IsTrue(IsJsonValid(_productSchemaJson, actualProduct),
                      "Expected to get same json schema as product has");
        Assert.That.IsSameProduct(product, actualProduct);
    }

    [TestMethod]
    public async Task Add_Product_With_Less_Than_Minimum_Category_Id()
    {
        // arrange

        // act
        var result = await _api.AddProduct(_addProductTestsJson["invalidByCategoryIdLess"]!.ToObject<Product>()!);

        // assert
        var productId = result["id"]!.ToObject<int>();
        _testProductsIds.Add(productId);
        Assert.IsNull(Product.GetProductById(productId, await _api.GetProducts()),
                      "Expected to not find recently created invalid product in product list");
    }

    public async Task Add_Product_With_More_Than_Maximim_Category_Id()
    {
        // arrange

        // act
        var result = await _api.AddProduct(_addProductTestsJson["invalidByCategoryIdMore"]!.ToObject<Product>()!);

        // assert
        var productId = result["id"]!.ToObject<int>();
        _testProductsIds.Add(productId);
        Assert.IsNull(Product.GetProductById(productId, await _api.GetProducts()),
                      "Expected to not find recently created invalid product in product list");
    }

    [TestMethod]
    public async Task Add_Products_With_Same_Title()
    {
        // arrange
        List<string> expectedAliases = new() { "chasy", "chasy-0", "chasy-0-0" };
        List<int> actualProductsId = new();
        List<string> actualAliases = new();
        var product = _addProductTestsJson["validWithWatchTitle"]!;

        // act
        foreach (var _ in expectedAliases)
        {
            var response = await _api.AddProduct(product.ToObject<Product>()!);
            var productId = response["id"]!.ToObject<int>();
            _testProductsIds.Add(productId);
            actualProductsId.Add(productId);
        }

        // assert
        var products = await _api.GetProducts();

        foreach (var id in actualProductsId)
        {
            var actualProduct = Product.GetProductById(id, products)!;
            Assert.That.IsSameProduct(product, actualProduct);
            actualAliases.Add(actualProduct["alias"]!.ToString());
        }

        Assert.AreEqual(expectedAliases[0], actualAliases[0], "Expected to get alias without -0 postfix");
        Assert.AreEqual(expectedAliases[1], actualAliases[1], "Expected to get alias with signle -0 postfix");
        Assert.AreEqual(expectedAliases[2], actualAliases[2], "Expected to get alias with double -0-0 postfix");
    }

    [TestMethod]
    public async Task Edit_Existing_Product()
    {
        // arrange
        var response = await _api.AddProduct(_addProductTestsJson["validProductForEditing"]!.ToObject<Product>()!);
        var productId = response["id"]!.ToObject<int>();
        _testProductsIds.Add(productId);
        var editProduct = _editProductTestsJson["valid"]!;
        editProduct["id"] = productId;
        var expectedAlias = editProduct["title"]!.ToString().ToLower();

        // act
        await _api.EditProduct(editProduct.ToObject<Product>()!);

        // assert
        var actualProduct = Product.GetProductById(productId, await _api.GetProducts())!;
        Assert.That.IsSameProduct(editProduct, actualProduct);
        Assert.AreEqual(expectedAlias, actualProduct["alias"], "Expected to get alias as lower version of title");
    }

    [TestMethod]
    public async Task Edit_Not_Existing_Product()
    {
        // arrange
        var expectedStatus = 1;
        var expectedProduct = _editProductTestsJson["validWithNotExistingId"]!;
        var expectedAlias = expectedProduct["title"]!.ToString().ToLower();

        // act
        var response = await _api.EditProduct(expectedProduct.ToObject<Product>()!);

        // assert
        var editedProduct = (await _api.GetProducts()).Last();
        _testProductsIds.Add(editedProduct["id"]!.ToObject<int>());

        Assert.That.IsSameProduct(expectedProduct, editedProduct);
        Assert.AreEqual(expectedAlias, editedProduct["alias"], "Expected to get alias as lower version of title");
        Assert.AreEqual(expectedStatus, response["status"]!, "Expected to receive successfull status code after edit");
    }

    [TestMethod]
    public async Task Edit_Product_Without_Id()
    {
        // arrange
        var expectedStatus = 0;

        // act
        var response = await _api.EditProduct(_editProductTestsJson["validWithoutId"]!.ToObject<Product>()!);

        // assert
        Assert.AreEqual(expectedStatus, response["status"]!.ToObject<int>(),
                        "Expected to get unsuccessfull status after trying editing product without specified id");
    }

    [TestMethod]
    public async Task Edit_Product_With_Same_Title_Twice()
    {
        var editProduct = _addProductTestsJson["validProductForEditing"]!;
        var response = await _api.AddProduct(editProduct.ToObject<Product>()!);
        var productId = response["id"]!.ToObject<int>();
        var expectedAliasAfterFirstEdit = $"{editProduct["title"]!.ToString().ToLower()}-{productId}";
        var expectedAliasAfterSecondEdit = editProduct["title"]!.ToString().ToLower();
        editProduct["id"] = productId;
        _testProductsIds.Add(productId);

        // edit with same data, especially with the same title at first time will ad -id postfix to the alias
        await _api.EditProduct(editProduct.ToObject<Product>()!);

        var actualProduct = Product.GetProductById(productId, await _api.GetProducts());
        Assert.IsNotNull(actualProduct);
        Assert.That.IsSameProduct(editProduct, actualProduct);
        Assert.AreEqual(expectedAliasAfterFirstEdit, actualProduct["alias"]);

        // edit with same data, especially with the same title twice in a row will remove -id postfix from the alias
        await _api.EditProduct(editProduct.ToObject<Product>()!);
        actualProduct = Product.GetProductById(productId, await _api.GetProducts());
        Assert.IsNotNull(actualProduct);
        Assert.That.IsSameProduct(editProduct, actualProduct);
        Assert.AreEqual(expectedAliasAfterSecondEdit, actualProduct["alias"]);
    }

    [TestMethod]
    public async Task Delete_Product()
    {
        var expectedStatus = 1;
        var response = await _api.AddProduct(_addProductTestsJson["validProductForEditing"]!.ToObject<Product>()!);
        var productId = response["id"]!.ToObject<int>();
        _testProductsIds.Add(productId);

        // delete
        response = await _api.DeleteProduct(productId);

        // make sure that product was deleted
        Assert.AreEqual(expectedStatus, response["status"]!.ToObject<int>(),
                        "Expected to get 0 status after trying editing product without specified id");
        Assert.IsNull(Product.GetProductById(productId, await _api.GetProducts()),
                      "Expected to not find recently deleted product in product list");
    }

    [TestMethod]
    public async Task Delete_Not_Existing_Product()
    {
        var expectedStatus = 0;
        var productId = 77777;

        // try to delete
        var result = await _api.DeleteProduct(productId);

        // make sure that there is no new product in product list
        Assert.AreEqual(expectedStatus, result["status"]!.ToObject<int>(),
                        "Expected to get 0 status after trying deleting not existing product");
        Assert.IsNull(Product.GetProductById(productId, await _api.GetProducts()),
                      "Expected to not find non-existent in product list");
    }
}
