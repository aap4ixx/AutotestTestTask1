using Dynatech.Test.Common.Extensions;
using OpenQA.Selenium;

namespace Dynatech.Test.PageObject; 

public static class AcceptCookiesPage {
    public static void AcceptAllCookies(IWebDriver session) {
        var acceptAllCookiesButton = "//button[text()='Accept All Cookies']".WaitForElement(session);
        acceptAllCookiesButton.ClickWrapper(session);
    }
}