using AuthenticatedLadder.Middlewares.ErrorHandling;
using Microsoft.AspNetCore.Builder;

namespace AuthenticatedLadder.ExtensionMethods
{
    public static class ErrorHandlingMiddlewareExtensions
    {
        public static void UseErrorHandlingMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
