using System;
using OpenQA.Selenium;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UITests.Order.DriverMethods;
using System.Threading;

namespace ShopTests.UI.MakingOrder.Tests
{
[TestClass]
public class MakeOrderTests
{
    private MakeOrderMethods _makeOrderMethods;
    private IWebDriver _webDriver;
    private string _url = "http://shop.qatl.ru/";
    public TestContext TestContext { get; set; }

    [TestInitialize]
    public void TestInitialize()
    {
        _webDriver = new OpenQA.Selenium.Chrome.ChromeDriver(Environment.GetEnvironmentVariable("CHROME_DIR"));
        _webDriver.Navigate().GoToUrl(_url);

        _makeOrderMethods = new MakeOrderMethods(_webDriver);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _webDriver.Quit();
    }

    [TestMethod]
    [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "..\\..\\Order\\Configs\\OrderCases.xml",
                "TestNotAuthorized", DataAccessMethod.Sequential)]
    public void MakeOrder()
    {
        var productLink = TestContext.DataRow["Link"].ToString();
        _webDriver.Navigate().GoToUrl($"{_url}{productLink}");

        _makeOrderMethods.SetCurrentElement(_makeOrderMethods.GetSimpleCartElement());
        var quantity = TestContext.DataRow["Quantity"].ToString();
        _makeOrderMethods.AddProductToCart(int.Parse(quantity));

        _makeOrderMethods.SwitchToCartPage(_url);

        var customerInfo =
            new CustomerInfo(TestContext.DataRow["Login"].ToString() + DateTime.Now.ToString("hh-mm-ss"),
                             TestContext.DataRow["Password"].ToString(), TestContext.DataRow["CustomerName"].ToString(),
                             DateTime.Now.ToString("hh-mm-ss") + TestContext.DataRow["Email"].ToString(),
                             TestContext.DataRow["Address"].ToString(), TestContext.DataRow["Note"].ToString());

        _makeOrderMethods.SubmitCustomerInfo(customerInfo);

        Assert.AreEqual(TestContext.DataRow["Alert"].ToString(), _makeOrderMethods.GetOrderAlertText(),
                        "Expected to see text according to the user submit info text");
    }
}
}
