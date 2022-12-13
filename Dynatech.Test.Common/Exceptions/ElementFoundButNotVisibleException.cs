using OpenQA.Selenium;

namespace Dynatech.Test.Common.Exceptions {
    public class ElementFoundButNotVisibleException : Exception {
        public ElementFoundButNotVisibleException(int id, By path) : base($"uniqueId: '{id}'\r\n" + $"By: '{path.Criteria}'") {
        }

        public ElementFoundButNotVisibleException(int id, string xPath, string errorMessage) : base($"uniqueId: '{id}'\r\n" +
                                                                                                    $"XPath: '{xPath}'; errorMessage: {errorMessage}") {
        }
    }
}