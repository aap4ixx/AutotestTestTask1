using OpenQA.Selenium;

namespace Dynatech.Test.Common.Exceptions {
    public class ElementFoundBytNotDisappear : Exception {
        public ElementFoundBytNotDisappear(int uniqueId, By by, int retries) : base(
            $"uniqueId: '{uniqueId}'\r\nretries: {retries}\r\nThe element will find and not disappear. By: '{by}''") {
        }
    }
}