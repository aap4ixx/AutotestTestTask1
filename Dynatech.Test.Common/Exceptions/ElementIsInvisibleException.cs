namespace Dynatech.Test.Common.Exceptions {
    public class ElementIsInvisibleException: Exception {
        public ElementIsInvisibleException(int id, string errorMessage): base($"uniqueId: '{id}'\r\n{errorMessage}") {
        }
    }
}