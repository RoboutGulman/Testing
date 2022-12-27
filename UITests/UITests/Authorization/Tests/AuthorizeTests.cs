
using System;
using UITests.Authorization.WebDriverMethods;
using OpenQA.Selenium;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UITests.Authorization.Tests
{
[TestClass]
public class AuthorizeTests
{
    private AuthorizeMethods _authorizeMethods;
    private IWebDriver _webDriver;
    private string _url = "http://shop.qatl.ru/";
    private string _loginUrl = "user/login";
    public TestContext TestContext { get; set; }

    [TestInitialize]
    public void TestInitialize()
    {
        _webDriver = new OpenQA.Selenium.Chrome.ChromeDriver(Environment.GetEnvironmentVariable("CHROME_DIR"));
        _webDriver.Navigate().GoToUrl(_url);

        _authorizeMethods = new AuthorizeMethods(_webDriver);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _webDriver.Quit();
    }

    [TestMethod]
    [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "..\\..\\Authorization\\Configs\\AuthorizeCases.xml",
                "Test", DataAccessMethod.Sequential)]
    public void Authorize()
    {
        var drop = _authorizeMethods.GetDropElement();
        _authorizeMethods.SetCurrentElement(drop);

        var dropdownMenu = _authorizeMethods.GetDropdownMenuElement();
        Assert.IsFalse(dropdownMenu.Displayed, "Expected not to see login dropdown after opening shop page");

        _authorizeMethods.ToggleAccountDropdownElement();
        Assert.IsTrue(dropdownMenu.Displayed, "Expected to see login dropdown after toggling account button");
        _authorizeMethods.SetCurrentElement(dropdownMenu);

        _authorizeMethods.ClickOnSignInButton();
        Assert.AreEqual(_url + _loginUrl, _webDriver.Url, "Expected to switch to the login page");

        _authorizeMethods.SetCurrentElement(_authorizeMethods.GetLoginFormElement());

        _authorizeMethods.SubmitUserInfo(TestContext.DataRow["Login"].ToString(),
                                         TestContext.DataRow["Password"].ToString());

        string expectedMessage = TestContext.DataRow["ExpectedMessage"].ToString();
        var actualMessage = _authorizeMethods.GetAlertMessageElement().Text;
        Assert.AreEqual(expectedMessage, actualMessage, "Expected to get alert message that corresponds user data");
    }
}
}
