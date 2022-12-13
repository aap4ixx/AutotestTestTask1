using Dynatech.Test.Common;
using Dynatech.Test.Common.Exceptions;
using Dynatech.Test.Common.Extensions;
using Dynatech.Test.Common.FixtureFile;

namespace Dynatech.Test.PageObject; 

public class BookFlight: FixtureSessionBase {
    
    public BookFlight(FixtureSessionBase fixtureSessionBase): base(fixtureSessionBase) {
    }
    
    private HtmlSelect PassengerInfoText => new(Session, "//*[text()='Passenger info']");
    private HtmlSelect PaymentInfoText => new(Session, "//*[text()='Payment Info']");
    private HtmlSelect BookPay => new(Session, "//button[text()='Book & Pay']");
    private HtmlSelect PriceSummary => new(Session, "//*[text()='Price summary']");
    
    public void WaitPageLoad() {
        PassengerInfoText.AssertDisplayedShouldBeTrue("Check on book flight have text 'Passenger info'");
        PaymentInfoText.AssertDisplayedShouldBeTrue("Check on book flight have text 'Payment Info'");
        BookPay.AssertDisplayedShouldBeTrue("Check on book flight have text 'Book & Pay'");
        PriceSummary.AssertDisplayedShouldBeTrue("Check on book flight have text 'Price summary'");
    }

    public void ComparePriceSummary(string expectedPrice) {
        var actualPrice = "//*[@data-qa='_totPrice']".WaitForElement(Session).GetFullText();

        if (expectedPrice != actualPrice) {
            var errorDescription = $"Price in result page do not match with price in booking page.\r\nExpected:'{expectedPrice}'\r\nActual:'{actualPrice}'";
            var uniqueId = ArtifactLogs.CreateArtifactLog("ComparePriceMatches", errorDescription, Session);
            throw new ValueCompareException(uniqueId, errorDescription);
        }
    }
}