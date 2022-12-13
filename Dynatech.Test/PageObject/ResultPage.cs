using Dynatech.Test.Common;
using Dynatech.Test.Common.Extensions;
using Dynatech.Test.Common.FixtureFile;

namespace Dynatech.Test.PageObject {
    public class ResultPage: FixtureSessionBase {
        public ResultPage(FixtureSessionBase fixtureSessionBase): base(fixtureSessionBase) {
        }

        //private HtmlSelect RootResultElement => new(Session, "//*[@class='stretch' and .//div[@data-qa='pqDateFrom']]");
        private HtmlSelect RootResultElement => new(Session, "//div[./button[text()='Show more results']]");
        
        
        public void WaitPageLoad() {
            Logger.Information("What when search bar finish load");
            "//*[@id='progress-bar-container']/div[@id='nprogress']".WaitElementDisappear(Session);
            
            Logger.Information("Wait when show button 'Show more results'");
            "//button[text()='Show more results']".WaitForElement(Session, retries: 60);
        }

        public void OpenFlight(int flightPositionNumber) {
            var flightBoxButton = $"({RootResultElement.XPath}/div[not(@class)])[{flightPositionNumber}]//button[text()='View details']".WaitForElement(Session);
            flightBoxButton.ClickWrapper(Session);
        }
    }
}