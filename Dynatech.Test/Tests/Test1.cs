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
    public class test1 : FixtureSessionBase, IDisposable {
        public test1(FixtureTests fixture, ITestOutputHelper output) : base(fixture, output) {
        }

        public void Dispose() {
        }

        readonly DateTime searchDate = new DateTime(2023, 01, 07);

        [Fact]
        [Documentation("Check all flight contains price and departure date")]
        public void Search_CheckPrice_CheckDepartureDate() {
            Logger.Information("Navigate to result page");
            var resultPageUrl = $"https://www.oojo.com/result/NYC-LHE/{searchDate:yyyy-MM-dd}/business";
            Session.GoToPage(resultPageUrl);

            Logger.Information("Wait when search is finish");
            var resultPage = new ResultPage(this);
            resultPage.WaitPageLoad();

            Logger.Information("Read all data from result from");
            var searchResultBusiness = new SearchResultBusiness(this);
            searchResultBusiness.ReadData();
            
            Logger.Information("Assert that every found flight contains price & Departure date is the same as from date which is in the URL");
            searchResultBusiness.CheckDepartureDate(searchDate);
        }
    }
}