using System;
using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace UITests.ProductSearch.DriverMethods
{
internal class SearchMethods
{
    private readonly IWebDriver _webDriver;
    private IWebElement _currentElement;

    private static readonly By _searchBreadCrumbXPath = By.XPath("//*[@class='breadcrumb']"),
                               _searchFormXPath = By.XPath("//form[@action='search']"),
                               _searchInputXPath = By.XPath(".//*[@id='typeahead']"),
                               _searchMenuSuggestionXPath = By.XPath(".//div[contains(@class,'tt-suggestion')]"),
                               _searchMenuXPath = By.XPath(".//div[contains(@class,'tt-menu')]"),
                               _searchOpenMenuXPath = By.XPath(".//div[contains(@class,'tt-open')]"),
                               _searchResultLinkXPath = By.XPath(".//a[contains(@href, 'product/')]"),
                               _searchSubmitInputXPath = By.XPath("//input[contains(@class,'tt-hint')]");

    public SearchMethods(IWebDriver webDriver)
    {
        _webDriver = webDriver;
    }

    public void SetCurrentElement(IWebElement webElement)
    {
        _currentElement = webElement;
    }

    public IWebElement GetSearchFormElement()
    {
        return _webDriver.FindElement(_searchFormXPath);
    }

    public IWebElement GetSearchInputElement()
    {
        return _webDriver.FindElement(_searchInputXPath);
    }

    public void FillInputElement(string searchQuery)
    {
        _currentElement.SendKeys(searchQuery);
        WebDriverWait wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(10));
        wait.Until(e => _webDriver.FindElement(_searchFormXPath).FindElement(_searchOpenMenuXPath).Displayed);
    }

    public IWebElement GetSearchMenuElement()
    {
        return _currentElement.FindElement(_searchMenuXPath);
    }

    public int GetSearchMenuSuggesstionsCount()
    {
        var menuDataset = _currentElement.FindElements(_searchMenuSuggestionXPath);
        return menuDataset.Count;
    }

    public void SubmitSearchInput()
    {
        _currentElement.FindElement(_searchSubmitInputXPath).SendKeys(Keys.Enter);
    }

    public string GetBreadCrumbText()
    {
        var breadCrumb = _webDriver.FindElement(_searchBreadCrumbXPath);
        return breadCrumb.Text;
    }

    public ReadOnlyCollection<IWebElement> GetSearchResults()
    {
        return _webDriver.FindElements(_searchResultLinkXPath);
    }
}
}
