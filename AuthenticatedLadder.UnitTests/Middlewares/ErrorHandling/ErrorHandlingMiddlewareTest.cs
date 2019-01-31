using AuthenticatedLadder.Middlewares.ErrorHandling;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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

        public ErrorHandlingMiddlewareTest()
        {
            _context = new DefaultHttpContext();
        }

        [Fact]
        public async Task InvokeAsync_ShouldCatchExceptionAndReturnStandardErrorWhenDelegateThrows()
        {
            RequestDelegate next = (innerHttpContext)
                => throw new Exception("This message must not be shown");

            _context.Response.Body = new MemoryStream();

            var middleware = new ErrorHandlingMiddleware(next);

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
        }
    }
}
