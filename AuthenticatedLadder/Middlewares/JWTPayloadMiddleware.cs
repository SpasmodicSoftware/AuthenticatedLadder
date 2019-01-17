using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using AuthenticatedLadder.Services.TokenDecoder;

namespace AuthenticatedLadder.Middlewares
{
    public class JWTPayloadMiddleware
    {
        private readonly RequestDelegate _next;
        private ITokenDecoderService _decoder;

        public JWTPayloadMiddleware(RequestDelegate next, ITokenDecoderService decoder)
        {
            _next = next;
            _decoder = decoder;
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
