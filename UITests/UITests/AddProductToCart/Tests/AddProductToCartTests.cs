using System;
using UITests.AddProductToCart.WebDriverMethods;
using OpenQA.Selenium;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UITests.AddProductToCart.Tests
{
internal static class CustomCartAssert
{
    public static void IsSameCartProduct(this Assert assert, CartProduct expected, CartProduct actual)
    {
        Assert.AreEqual(expected.name.ToLower(), actual.name.ToLower(), "Expected to get same product names");
        Assert.AreEqual(expected.quantity, string.IsNullOrEmpty(actual.quantity) ? "0" : actual.quantity,
                        "Expected to get same product quantities");
        Assert.AreEqual(expected.price, actual.price, "Expected to get same product prices");
    }

    public static void IsSameCartTotal(this Assert assert, CartTotal expected, CartTotal actual)
    {
        Assert.AreEqual(expected.quantity, string.IsNullOrEmpty(actual.quantity) ? "0" : actual.quantity,
                        "Expected to get same cart quantities");
        Assert.AreEqual(expected.price, actual.price, "Expected to get same cart prices");
    }
}

[TestClass]
public class AddToCartTests
{
    private static readonly int COMMON_PRODUCT_COUNT = 1;
    private AddToCartMethods _addToCartMethods;
    private IWebDriver _webDriver;
    private string _url = "http://shop.qatl.ru/";
    public TestContext TestContext { get; set; }

    [TestInitialize]
    public void TestInitialize()
    {
        _webDriver = new OpenQA.Selenium.Chrome.ChromeDriver(Environment.GetEnvironmentVariable("CHROME_DIR"));
        _webDriver.Navigate().GoToUrl(_url);

        _addToCartMethods = new AddToCartMethods(_webDriver);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _webDriver.Quit();
    }

    [TestMethod]
    [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "..\\..\\AddProductToCart\\Configs\\AddProductToCartCases.xml",
                "TestProductPage", DataAccessMethod.Sequential)]
    public void AddToCartFromProductPage()
    {
        var productLink = TestContext.DataRow["Link"].ToString();
        _webDriver.Navigate().GoToUrl($"{_url}{productLink}");

        _addToCartMethods.SetCurrentElement(_addToCartMethods.GetSimpleCartElement());
        var quantity = TestContext.DataRow["Quantity"].ToString();
        _addToCartMethods.AddProductToCart(int.Parse(quantity));

        _addToCartMethods.SetCurrentElement(_addToCartMethods.GetModalElement());

        var cartProduct = _addToCartMethods.GetCartProduct(0);

        var name = TestContext.DataRow["Name"].ToString();
        var price = TestContext.DataRow["Price"].ToString();

        Assert.That.IsSameCartProduct(new CartProduct(name, quantity, price), cartProduct);

        var cartTotal = _addToCartMethods.GetCartTotal();
        var totalQuantity = TestContext.DataRow["TotalQuantity"].ToString();
        var totalPrice = TestContext.DataRow["TotalPrice"].ToString();
        Assert.That.IsSameCartTotal(new CartTotal(totalQuantity, totalPrice), cartTotal);

        Assert.AreEqual(totalPrice, _addToCartMethods.GetSimpleCartTotal());
    }

    [TestMethod]
    [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "..\\..\\AddProductToCart\\Configs\\AddProductToCartCases.xml",
                "TestProductList", DataAccessMethod.Sequential)]
    public void AddToCartFromProductList()
    {
        CartProduct firstProduct = BuildAndAddProductToCart("First");
        _addToCartMethods.CloseModal();
        CartProduct secondProduct = BuildAndAddProductToCart("Second");

        var firstCartProduct = _addToCartMethods.GetCartProduct(0);
        var secondCartProduct = _addToCartMethods.GetCartProduct(1);

        Assert.That.IsSameCartProduct(firstProduct, firstCartProduct);
        Assert.That.IsSameCartProduct(secondProduct, secondCartProduct);

        var cartTotal = _addToCartMethods.GetCartTotal();
        var totalQuantity = TestContext.DataRow["TotalQuantity"].ToString();
        var totalPrice = TestContext.DataRow["TotalPrice"].ToString();
        Assert.That.IsSameCartTotal(new CartTotal(totalQuantity, totalPrice), cartTotal);
    }

    private CartProduct BuildAndAddProductToCart(string dataRowPrefix)
    {
        CartProduct product = new CartProduct(TestContext.DataRow[$"{dataRowPrefix}ProductName"].ToString(),
                                              COMMON_PRODUCT_COUNT.ToString(),
                                              TestContext.DataRow[$"{dataRowPrefix}ProductPrice"].ToString());
        var productLink = TestContext.DataRow[$"{dataRowPrefix}ProductLink"].ToString();
        _addToCartMethods.SetCurrentElement(_addToCartMethods.GetProductItemCartElement(productLink));
        _addToCartMethods.AddProductToCart(COMMON_PRODUCT_COUNT);

        return product;
    }
}
}
