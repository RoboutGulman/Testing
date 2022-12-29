using System;
using UITests.AddProductToCart.DriverMethods;
using OpenQA.Selenium;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UITests.AddProductToCart.Tests
{
internal static class CustomCartAssert
{
    public static void IsSameCartProductInfo(this Assert assert, CartProductInfo expected, CartProductInfo actual)
    {
        Assert.AreEqual(expected.name.ToLower(), actual.name.ToLower(), "Expected to get same product names");
        Assert.AreEqual(expected.quantity, string.IsNullOrEmpty(actual.quantity) ? "0" : actual.quantity,
                        "Expected to get same product quantities");
        Assert.AreEqual(expected.price, actual.price, "Expected to get same product prices");
    }

    public static void IsSameCartTotalInfo(this Assert assert, CartTotalInfo expected, CartTotalInfo actual)
    {
        Assert.AreEqual(expected.quantity, string.IsNullOrEmpty(actual.quantity) ? "0" : actual.quantity,
                        "Expected to get same cart quantities");
        Assert.AreEqual(expected.price, actual.price, "Expected to get same cart prices");
    }
}

[TestClass]
public class AddToCartTests
{
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
    [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
                "..\\..\\AddProductToCart\\Configs\\AddProductToCartCases.xml", "TestProductPage",
                DataAccessMethod.Sequential)]
    public void AddProductToCart()
    {
        var productLink = TestContext.DataRow["Link"].ToString();
        _webDriver.Navigate().GoToUrl($"{_url}{productLink}");

        _addToCartMethods.SetCurrentElement(_addToCartMethods.GetSimpleCartElement());
        var quantity = TestContext.DataRow["Quantity"].ToString();
        _addToCartMethods.AddProductToCart(int.Parse(quantity));

        _addToCartMethods.SetCurrentElement(_addToCartMethods.GetModalElement());

        Assert.That.IsSameCartProductInfo(new CartProductInfo(TestContext.DataRow["Name"].ToString(), quantity,
                                                          TestContext.DataRow["Price"].ToString()),
                                      _addToCartMethods.GetCartProduct(0));

        var totalPrice = TestContext.DataRow["TotalPrice"].ToString();
        Assert.That.IsSameCartTotalInfo(new CartTotalInfo(TestContext.DataRow["TotalQuantity"].ToString(), totalPrice),
                                    _addToCartMethods.GetCartTotalInfo());

        Assert.AreEqual(totalPrice, _addToCartMethods.GetSimpleCartTotal());
    }
}
}
