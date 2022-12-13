using Dynatech.Test.Common.Extensions;
using OpenQA.Selenium;

namespace Dynatech.Test.Common {
    public class HtmlSelect {
        public string XPath { get; }
        private readonly IWebDriver session;
        private readonly bool checkOnDisplayed;
        public HtmlSelect(IWebDriver session, string xPath, bool checkOnDisplayed = true) {
            XPath = xPath;
            this.session = session;
            this.checkOnDisplayed = checkOnDisplayed;
        }

        public IWebElement GetElement() {
            return XPath.WaitForElement(session, checkOnDisplayed);
        }
        
        public void AssertDisplayedShouldBeTrue(string errorMessage) {
            var errorMessageList = new List<string> { errorMessage };
            XPath.WaitForElement(session, checkOnDisplayed, extraErrorDescriptionToLog: errorMessageList);
        }

        public void WaitElementHide() {
            XPath.WaitElementHide(session);
        }

        public HtmlSelect AppendXpath(string appendXpath) {
            return new HtmlSelect(session, XPath + appendXpath);
        }

        public string GetXpath() {
            return XPath;
        }
        public void ClickWrapper() {
            var button = GetElement();
            button.ClickWrapper(session);
        }
    }
}