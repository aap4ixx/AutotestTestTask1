namespace Dynatech.Test.Common.Exceptions {
    public class ElementNotHideException: Exception {
        public ElementNotHideException(int uniqueId, string errorDescription): base($"uniqueId: {uniqueId}\r\nError description:\r\n{errorDescription}\r\n") {
        }
    }
}