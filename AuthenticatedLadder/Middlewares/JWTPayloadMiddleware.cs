using AuthenticatedLadder.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticatedLadder.Middlewares
{
    public class JWTPayloadMiddleware
    {
        private readonly RequestDelegate _next;

        public JWTPayloadMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ITokenDecoderService decoder)
        {
            //se decoder torna null manda 401
            //altrimenti scrivi il payload in una variabile del context.Items
            await _next(context);
            //fare cose fattibili nella pipeline di ritorno
        }
    }
}
