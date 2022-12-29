using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using OpenQA.Selenium;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UITests.ProductSearch.DriverMethods;

namespace ShopTests.UI.SearchingProductInCatalog.Tests
{
internal static class CustomWebElementAssert
{
    public static void IsEachWebElementEnabled(this Assert assert, ReadOnlyCollection<IWebElement> webElements)
    {
        foreach (var element in webElements)
        {
            Assert.IsTrue(element.Enabled, "Expected to see enabled web element");
        }
    }

    public static void IsAppropriateBreadCrumb(this Assert assert, string breadCrumb, List<string> expectedElements)
    {
        foreach (var element in expectedElements)
        {
            Assert.IsTrue(breadCrumb.Contains(element), "Expected to contain each element in bread crumb");
        }
    }
}

[TestClass]
public class SearchTests
{
    private SearchMethods _searchMethods;
    private IWebDriver _webDriver;
    private string _url = "http://shop.qatl.ru/";
    private string _searchUrl = "http://shop.qatl.ru/search/";
    public TestContext TestContext { get; set; }

    [TestInitialize]
    public void TestInitialize()
    {
        _webDriver = new OpenQA.Selenium.Chrome.ChromeDriver(Environment.GetEnvironmentVariable("CHROME_DIR"));
        _webDriver.Navigate().GoToUrl(_url);

        _searchMethods = new SearchMethods(_webDriver);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _webDriver.Quit();
    }

    [TestMethod]
    [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
                "..\\..\\ProductSearch\\Configs\\ProductSearchCases.xml", "TestSearch", DataAccessMethod.Sequential)]
    public void SearchProducts()
    {
        _searchMethods.SetCurrentElement(_searchMethods.GetSearchFormElement());
        var searchMenu = _searchMethods.GetSearchMenuElement();
        Assert.IsFalse(searchMenu.Displayed, "Expected not to see search menu when nothing was given to search");

        _searchMethods.SetCurrentElement(_searchMethods.GetSearchInputElement());
        string searchQuery = TestContext.DataRow["SearchQuery"].ToString();
        _searchMethods.FillInputElement(searchQuery);
        Assert.IsTrue(searchMenu.Displayed, "Expected to see search menu when text was given to search");

        int expectedSuggestionsCount = int.Parse(TestContext.DataRow["Count"].ToString());
        _searchMethods.SetCurrentElement(searchMenu);
        Assert.AreEqual(expectedSuggestionsCount, _searchMethods.GetSearchMenuSuggesstionsCount(),
                        $"Expected to get {expectedSuggestionsCount} number of suggestions");

        _searchMethods.SubmitSearchInput();
        Assert.AreEqual(_searchUrl, $"{_webDriver.Url.Split('?').First()}/",
                        "Expected to switch to search result page");
        Assert.That.IsAppropriateBreadCrumb(_searchMethods.GetBreadCrumbText(), new List<string> { searchQuery });
        Assert.That.IsEachWebElementEnabled(_searchMethods.GetSearchResults());
    }
}
}
