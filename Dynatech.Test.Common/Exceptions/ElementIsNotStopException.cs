namespace Dynatech.Test.Common.Exceptions {
    public class ElementIsNotStopException: Exception {
        public ElementIsNotStopException(int uniqueId, string errorMessage): base($"uniqueId: '{uniqueId}'\r\nElement is not stopException.\r\nError description: {errorMessage}") {
        }
    }
}