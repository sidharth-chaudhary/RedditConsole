using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace RedditConsole.ServiceRepository.Tests
{
    [TestClass()]
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

            configMock.SetupGet(x => x["OAuthToken"]).Returns("eyJhbGciOiJSUzI1NiIsImtpZCI6IlNIQTI1NjpzS3dsMnlsV0VtMjVmcXhwTU40cWY4MXE2OWFFdWFyMnpLMUdhVGxjdWNZIiwidHlwIjoiSldUIn0.eyJzdWIiOiJ1c2VyIiwiZXhwIjoxNzE0OTYyOTQ0LjE0NzUyOSwiaWF0IjoxNzE0ODc2NTQ0LjE0NzUyOSwianRpIjoiMXExa2VQNG9lRlRWV1ZSQzZYM3FocEFNenF0RGpnIiwiY2lkIjoiRTZUTm1nemY1a1VkNFFIYXBXZ1VLQSIsImxpZCI6InQyX3prNWs1b28xZSIsImFpZCI6InQyX3prNWs1b28xZSIsImxjYSI6MTcxNDY5ODU2MDYwOSwic2NwIjoiZUp5S1Z0SlNpZ1VFQUFEX193TnpBU2MiLCJmbG8iOjl9.C4gRzUG4d0bJvbQ2oVELg_tAthEdiabpfFPvbtzKjclFNYOx32W2CG2-WmzhCT0J640orYFRfJVMHANohPENJSWN4m82tmIZTRgteM4Qiph39uTDrr8sC6UyxNuUaKBObDWWLggoKgSM1dcTgjKk83-pwOzp7GslAZ1l1LMotemceDYWle7aXKDye3b0B-a6jlhtoH1SBXmT0ETUXxXtR7U0l0kbHWQcBd83p5oaLJcrZZOSCQwufMiXyDYGVbeG4N3FEresEzy6ifyBpg5UsY1iWSPikOG0xP1Kn0R4MNukn2UsooW9rz6pFuQpBehzaj-8gRlLScj-f0q2rKE_qQ");
            configMock.SetupGet(x => x["EscapeDataString"]).Returns("android:com.arnvanhoutte.redder:v1.2.3 (by /u/nerdiator)");
            configMock.SetupGet(x => x["RedditUri"]).Returns("https://oauth.reddit.com/r/worldnews/top.json?");
            configMock.SetupGet(x => x["RateLimit"]).Returns("100");
            configMock.SetupGet(x => x["RefreshAPI"]).Returns("10000"); 

            var service = new SubRedditServiceRepo(loggerMock.Object, configMock.Object);

            // Act
            await service.GetSubredditApiAsync();

            // Assert
            // Verify that the logger was called with the expected message.
            //Test email notification during push
            loggerMock.Verify(x => x.LogInformation("Get API call to Subreddit-worldnews is successful."), Times.Once);
        }

    }
}