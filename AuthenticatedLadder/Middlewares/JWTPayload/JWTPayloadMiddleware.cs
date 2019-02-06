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

        public JWTPayloadMiddleware(RequestDelegate next, ITokenDecoderService decoder, IOptions<JWTPayloadMiddlewareSettings> settings)
        {
            _next = next;
            _decoder = decoder;
            _settings = settings.Value;
        }

        public async Task InvokeAsync(HttpContext context, ITokenDecoderService decoder, IJWTPayloadHolder payloadHolder)
        {
            var payload = _decoder.Decode(_settings.DecodeSecret, GetBearerToken(context.Request));
            if (payload != null)
            {
                payloadHolder.HoldPayload("JWTSignedPayload", payload);

                await _next(context);
            }
            else
            {
                context.Response.StatusCode = 401;
            }
        }

        private string GetBearerToken(HttpRequest request)
        {
            return request.Headers[_settings.HeaderName].ToString();
        }
    }
}
