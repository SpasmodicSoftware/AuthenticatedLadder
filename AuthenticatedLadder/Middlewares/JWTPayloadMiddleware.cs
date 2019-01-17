using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using AuthenticatedLadder.Services.TokenDecoder;
using Microsoft.Extensions.Options;

namespace AuthenticatedLadder.Middlewares
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

        public async Task Invoke(HttpContext context, ITokenDecoderService decoder)
        {
            var payload = _decoder.Decode(GetBearerToken(context.Request));
            if (payload != null)
            {
                context.Items["JWTSignedPayload"] = payload;

                await _next(context);
            }
            else
            {
                context.Response.StatusCode = 401;
            }


        }

        private string GetBearerToken(HttpRequest request)
        {
            var authorizationHeaderContent = request.Headers["Authorization"].ToString().Split(' ');
            return authorizationHeaderContent.Length == 2 ? authorizationHeaderContent[1] : null;
        }
    }
}
