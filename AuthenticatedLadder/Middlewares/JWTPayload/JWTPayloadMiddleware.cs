using AuthenticatedLadder.Logging;
using AuthenticatedLadder.Services.JWTPayloadHolder;
using AuthenticatedLadder.Services.TokenDecoder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace AuthenticatedLadder.Middlewares.JWTPayload
{
    public class JWTPayloadMiddleware
    {
        private readonly RequestDelegate _next;
        private ITokenDecoderService _decoder;
        private JWTPayloadMiddlewareSettings _settings;
        private ILoggerAdapter<JWTPayloadMiddleware> _logger;

        public JWTPayloadMiddleware(RequestDelegate next, 
                ITokenDecoderService decoder, 
                IOptions<JWTPayloadMiddlewareSettings> settings,
                ILoggerAdapter<JWTPayloadMiddleware> logger)
        {
            _next = next;
            _decoder = decoder;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ITokenDecoderService decoder, IJWTPayloadHolder payloadHolder)
        {
            var token = GetBearerToken(context.Request);
            _logger.LogInformation($"Received token: {token}");
            var payload = _decoder.Decode(_settings.DecodeSecret, token);
            if (payload != null)
            {
                payloadHolder.HoldPayload("JWTSignedPayload", payload);

                await _next(context);
            }
            else
            {
                _logger.LogInformation("Invalid payload, you are not authorized");
                context.Response.StatusCode = 401;
            }
        }

        private string GetBearerToken(HttpRequest request)
        {
            return request.Headers[_settings.HeaderName].ToString();
        }
    }
}
