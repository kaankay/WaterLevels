using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using System.Net;
using WaterLevels;

namespace TestWaterLevels
{
    [Apartment(ApartmentState.STA)]
    public class Tests
    {
        private Mock<HttpMessageHandler> httpMessageHandlerMock;
        private HttpClient httpClient;
        MainWindow main;

        [SetUp]
        public void Setup()
        {
            httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            httpClient = new HttpClient(httpMessageHandlerMock.Object);
            main = new MainWindow();
        }

        [Test]
        public void TestGetAPI()
        {
            var responseBody = @"{{""uuid"": ""47174d8f-1b8e-4599-8a59-b580dd55bc87"",""number"": ""48900237"",""shortname"": ""EITZE"",""longname"": ""EITZE"",""km"": 9.56,""agency"": ""VERDEN"",""longitude"": 9.276769435375872,""latitude"": 52.90406544743417,""water"": {""shortname"": ""ALLER"",""longname"": ""ALLER""}}}";
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseBody)
            };

            httpMessageHandlerMock.Protected().Setup<HttpResponseMessage>("Send", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).Returns(responseMessage);

            string url = "https://www.pegelonline.wsv.de/webservices/rest-api/v2/stations/EITZE.json";
            dynamic result = main.GetAPI(url);

            string longname = ((JObject)result)["longname"]!.ToString();
            Assert.That(longname, Is.EqualTo("EITZE"));
        }
    }
}