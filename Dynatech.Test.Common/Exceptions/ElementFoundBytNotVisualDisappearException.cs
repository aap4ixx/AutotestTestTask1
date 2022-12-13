using OpenQA.Selenium;

namespace Dynatech.Test.Common.Exceptions {
    public class ElementFoundBytNotVisualDisappear: Exception {
        public ElementFoundBytNotVisualDisappear(int uniqueId, By by): base(
            $"uniqueId: '{uniqueId}'\r\nThe element will find and not disappear or not hide. By: '{by}''") {
        }
    }
}