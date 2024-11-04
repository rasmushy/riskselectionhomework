using Homework.Api.Tests.Helpers;

namespace Homework.Api.Tests.Factories
{
    /// <summary>
    /// Factory for creating instances of MockHttpMessageHandler with various configurations.
    /// </summary>
    public static class MockHttpMessageHandlerFactory
    {
        public static MockHttpMessageHandler Create(bool returnEmptyList = false)
        {
            return new MockHttpMessageHandler(returnEmptyList);
        }
    }
}
