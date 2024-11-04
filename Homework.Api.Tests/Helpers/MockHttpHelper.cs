using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace Homework.Api.Tests.Helpers
{
    public static class MockHttpHelper
    {
        public static void SetupHttpClientResponse(Mock<HttpMessageHandler> httpMessageHandlerMock, string content, HttpStatusCode statusCode)
        {
            var mockResponse = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
            };

            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse);
        }
    }
}
