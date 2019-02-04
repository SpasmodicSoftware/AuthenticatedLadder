using AuthenticatedLadder.Logging;
using AuthenticatedLadder.Middlewares.ErrorHandling;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace AuthenticatedLadder.UnitTests.Middlewares.ErrorHandling
{
    public class ErrorHandlingMiddlewareTest
    {
        private HttpContext _context;
        private Mock<ILoggerAdapter<ErrorHandlingMiddleware>> _logger;

        public ErrorHandlingMiddlewareTest()
        {
            _context = new DefaultHttpContext();
            _logger = new Mock<ILoggerAdapter<ErrorHandlingMiddleware>>();
        }

        [Fact]
        public async Task InvokeAsync_ShouldCatchExceptionAndReturnStandardErrorWhenDelegateThrows()
        {
            Task next(HttpContext innerHttpContext)
                => throw new Exception("This message must not be shown");

            _context.Response.Body = new MemoryStream();

            var middleware = new ErrorHandlingMiddleware(next, _logger.Object);

            await middleware.InvokeAsync(_context);

            _context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(_context.Response.Body);
            var streamText = reader.ReadToEnd();

            var result = JsonConvert.DeserializeObject<ErrorDetails>(streamText);

            var expected = new ErrorDetails
            {
                StatusCode = 500,
                Message = "Oooops! Something went wrong. Call Alessandro :)"
            };

            result
                .Should()
                .BeEquivalentTo(expected);

            _logger.Verify(l => l.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once());
        }
    }
}
