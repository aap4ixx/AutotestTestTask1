namespace Dynatech.Test.Common.Exceptions {
    public class ElementFromGroupNotFoundException: Exception {
        public ElementFromGroupNotFoundException(int id, string xPath1, string xPath2): base(
            $"uniqueId: '{id}'\r\nIn attempt to find error, error was not found\r\nxPath1:'{xPath1}'\r\nxPath2:'{xPath2}'") {
        }
    }
}