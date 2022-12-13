namespace Dynatech.Test.Common.Exceptions {
    public class ElementNotClickableException: Exception {
        public ElementNotClickableException(int uniqueId, int retries, string errorDescription): base($"uniqueId: '{uniqueId}'\r\n" + $"retries: '{retries}'\r\n{errorDescription}") {
        }
        
        public ElementNotClickableException(int uniqueId, string errorDescription): base($"uniqueId: '{uniqueId}'\r\n{errorDescription}") {
        }
    }
}