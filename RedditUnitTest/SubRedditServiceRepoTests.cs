using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using System.Threading;
using Moq;
using Xunit;
using Microsoft.Testing.Platform.Logging;
using RedditConsole.Model;


namespace RedditUnitTest
{
    [TestClass]
    public class SubRedditServiceRepoTests
    {

        [Fact]
        public async Task GetSubredditApiAsync_SuccessfulResponse()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<SubRedditServiceRepo>>();
            var configMock = new Mock<IConfiguration>();

            var httpClientHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            httpClientHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"data\": {\"children\": []}}")
                });

            var httpClient = new HttpClient(httpClientHandler.Object);

            configMock.SetupGet(x => x["OAuthToken"]).Returns("YourOAuthToken");
            configMock.SetupGet(x => x["EscapeDataString"]).Returns("YourEscapeDataString");
            configMock.SetupGet(x => x["RedditUri"]).Returns("YourRedditUri");
            configMock.SetupGet(x => x["RateLimit"]).Returns("YourRateLimit");
            configMock.SetupGet(x => x["RefreshAPI"]).Returns("60000"); // 1 minute

            var service = new SubRedditServiceRepo(loggerMock.Object, configMock.Object);

            // Act
            await service.GetSubredditApiAsync();

            // Assert
            // Verify that the logger was called with the expected message
            loggerMock.Verify(x => x.LogInformation("Get API call to Subreddit-worldnews is successful."), Times.Once);
        }

        // You can add more test cases for different scenarios (e.g., unsuccessful response, exceptions, etc.)
    }

}
