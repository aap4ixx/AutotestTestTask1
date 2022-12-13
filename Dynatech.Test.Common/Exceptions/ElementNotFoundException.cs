using OpenQA.Selenium;

namespace Dynatech.Test.Common.Exceptions {
    public class ElementNotFoundException: Exception {
        public ElementNotFoundException(string xPath): base($"Element not found.\r\nXPath: '{xPath}'") {
        }
        
        public ElementNotFoundException(int id, string xPath): base($"Element not found.\r\nuniqueId: '{id}'\r\nElement not found.\r\nXPath: '{xPath}'") {
        }
        
        public ElementNotFoundException(int id, int retry, By path): base($"Element not found.\r\nuniqueId: '{id}'\r\nretry: {retry}\r\nBy: '{path}'") {
        }
        
        public ElementNotFoundException(int id, int retry, string xPath): base($"Element not found.\r\nuniqueId: '{id}'\r\nretry: {retry}\r\nxPath: '{xPath}'") {
        }
    }
}