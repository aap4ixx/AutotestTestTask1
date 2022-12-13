namespace Dynatech.Test.Common.Exceptions {
    public class ElementIsVisibleException: Exception {
        public ElementIsVisibleException(int id, string errorMessage): base($"uniqueId: '{id}'\r\n{errorMessage}") {

        }
    }
}