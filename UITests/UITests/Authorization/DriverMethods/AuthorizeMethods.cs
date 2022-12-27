using OpenQA.Selenium;

namespace UITests.Authorization.WebDriverMethods
{
internal class AuthorizeMethods
{
    private readonly IWebDriver _webDriver;
    private IWebElement _currentElement;

    private static readonly By _alertMessageXPath = By.XPath("//div[contains(@class,'alert')]"),
                               _authFormXPath = By.XPath("//form[@id='login']"),
                               _authInputXPath = By.XPath(".//input[@id='login']"),
                               _authPasswordInputXPath = By.XPath(".//input[@id='pasword']"),
                               _authSubmitButtonXPath = By.XPath(".//button[@type='submit']"),
                               _dropdownAccountXPath = By.XPath(".//*[@class='dropdown-toggle']"),
                               _dropdownMenuXPath = By.XPath(".//*[contains(@class,'dropdown-menu')]"),
                               _dropdownSignInButtonXPath = By.XPath($".//a[contains(@href,'user/login')]"),
                               _dropXPath = By.XPath("//div[contains(@class,'drop')]");

    public AuthorizeMethods(IWebDriver webDriver)
    {
        _webDriver = webDriver;
    }

    public void SetCurrentElement(IWebElement webElement)
    {
        _currentElement = webElement;
    }

    public IWebElement GetDropElement()
    {
        return _webDriver.FindElement(_dropXPath);
    }

    public IWebElement GetDropdownMenuElement()
    {
        return _currentElement.FindElement(_dropdownMenuXPath);
    }

    public void ToggleAccountDropdownElement()
    {
        var dropdownAccount = _currentElement.FindElement(_dropdownAccountXPath);
        dropdownAccount.Click();
    }

    public void ClickOnSignInButton()
    {
        var signInButton = _currentElement.FindElement(_dropdownSignInButtonXPath);
        signInButton.Click();
    }

    public IWebElement GetLoginFormElement()
    {
        return _webDriver.FindElement(_authFormXPath);
    }

    public void SubmitUserInfo(string login, string password)
    {
        _currentElement.FindElement(_authInputXPath).SendKeys(login);
        _currentElement.FindElement(_authPasswordInputXPath).SendKeys(password);
        _currentElement.FindElement(_authSubmitButtonXPath).Click();
    }

    public IWebElement GetAlertMessageElement()
    {
        return _webDriver.FindElement(_alertMessageXPath);
    }
}
}
