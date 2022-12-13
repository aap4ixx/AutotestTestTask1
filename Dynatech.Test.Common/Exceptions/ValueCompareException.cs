namespace Dynatech.Test.Common.Exceptions {
    public class ValueCompareException: Exception {
        public ValueCompareException(int uniqueId, string errorMessage): base($"uniqueId: {uniqueId}\r\n{errorMessage}") {
        }
    }
}