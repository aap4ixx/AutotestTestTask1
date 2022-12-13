using Dynatech.Test.Common.Extensions;
using Dynatech.Test.Common.FixtureFile;
using Dynatech.Test.Configs;
using Dynatech.Test.PageObject;
using Dynatech.Test.PageObject.SearchResultObjects;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Dynatech.Test.Tests {
    [Collection(CollectionTests.NAME)]
    public class Test2 : FixtureSessionBase, IDisposable {
        public Test2(FixtureTests fixture, ITestOutputHelper output) : base(fixture, output) {
        }

        public void Dispose() {
        }

        readonly DateTime searchDate = new DateTime(2023, 01, 07);

        [Fact]
        [Documentation("Search fly, check price, open pre book, check price, open book and check total price'")]
        public void Search_CheckPrice_OpenPreBook_CheckPrice_OpenBookFlight_CheckPrice() {
            Logger.Information("Navigate to result page");
            var resultPageUrl = $"https://www.oojo.com/result/NYC-LHE/{searchDate:yyyy-MM-dd}/business";
            Session.GoToPage(resultPageUrl);

            Logger.Information("Wait when search is finish");
            var resultPage = new ResultPage(this);
            resultPage.WaitPageLoad();

            Logger.Information("Read all data from result from");
            var searchResultBusiness = new SearchResultBusiness(this);
            searchResultBusiness.ReadData();

            Logger.Information("Check if the flight price is present");
            searchResultBusiness.CheckFlightPricePresent();

            Logger.Information("get price from box two for compare");
            var priceText = searchResultBusiness.GetPrice(2);

            Logger.Information("Open flight");
            resultPage.OpenFlight(2);

            Logger.Information("Wait when popup is open");
            var preBookPopup = new PreBookPopup(this);
            preBookPopup.WaitPageLoad();

            Logger.Information("Compare price matches");
            preBookPopup.ComparePriceMatches(priceText);

            Logger.Information("Click on book-flight");
            preBookPopup.ClickOnBookFlight();

            Logger.Information("Wait when book flight page is open");
            var bookFlight = new BookFlight(this);
            bookFlight.WaitPageLoad();

            Logger.Information("Compare price on booking page");
            bookFlight.ComparePriceSummary(priceText);
        }
    }
}