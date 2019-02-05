using AuthenticatedLadder.Middlewares.JWTPayload;
using Microsoft.AspNetCore.Builder;

namespace AuthenticatedLadder.ExtensionMethods
{
    public static class JWTPayloadMiddlewareExtensions
    {
        public static IApplicationBuilder UseJWTPayloadMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JWTPayloadMiddleware>();
        }
    }
}
