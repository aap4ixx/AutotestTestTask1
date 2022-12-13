namespace Dynatech.Test.Common.Exceptions {
    public class FocusToElementFailedException: Exception {
        public FocusToElementFailedException(int id): base($"uniqueId: '{id}'\r\nMoveFocusToElement which doesn't work.") {
        }
        
        public FocusToElementFailedException(int id, string errorDescription): base($"uniqueId: '{id}'\r\nMoveFocusToElement which doesn't work.;\r\nError Description: '{errorDescription}'") {
        }
    }
}