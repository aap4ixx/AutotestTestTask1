using Dynatech.Test.Common.Extensions;
using OpenQA.Selenium;

namespace Dynatech.Test.PageObject; 

public static class MemberSubscriptionPage {
    public static void ClickOnCross(IWebDriver session) {
        var memberSubscriptionCross = "//*[contains(@class, 'modalCustom_modal') and @role='dialog']/div".WaitForElement(session, retries: 60);
        memberSubscriptionCross.ClickWrapper(session);
    }
}