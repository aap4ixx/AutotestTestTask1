namespace Dynatech.Test.Common.Exceptions {
    public class CustomException : Exception {
        public CustomException(string errorMessage) : base(errorMessage) {
        }

        public CustomException(int uniqueId, string errorMessage) : base($"uniqueId: '{uniqueId}'\r\n{errorMessage}") {
        }
    }
}