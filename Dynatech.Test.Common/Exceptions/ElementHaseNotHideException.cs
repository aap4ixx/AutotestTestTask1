using OpenQA.Selenium;

namespace Dynatech.Test.Common.Exceptions {
    public class ElementHaseNotHideException: Exception {
        public ElementHaseNotHideException(int uniqueId, By by): base($"uniqueId: '{uniqueId}'\r\n" + $"Element not hide by time. By: '{by}'") {

        }
    }
}