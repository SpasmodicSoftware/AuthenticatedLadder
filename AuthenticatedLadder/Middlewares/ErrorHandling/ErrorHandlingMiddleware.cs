using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;

namespace AuthenticatedLadder.Middlewares.ErrorHandling
{
    public static class ErrorHandlingMiddleware
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app/*TODO, ILoggerManager logger*/)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        //TODO ADD LOG FOR EXCEPTION
                        //logger.LogError($"Something went wrong: {contextFeature.Error}");

                        await context.Response.WriteAsync(JsonConvert.SerializeObject(
                            new ErrorDetails()
                            {
                                StatusCode = context.Response.StatusCode,
                                Message = "Oooops! Something went wrong. Call Alessandro."
                            }
                            ));
                    }
                });
            });
        }
    }
}
