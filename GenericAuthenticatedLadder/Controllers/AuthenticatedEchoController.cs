using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericAuthenticatedLadder.Controllers
{
    public class AuthenticatedEchoController : ControllerBase
    {
        [Authorize, HttpGet("echo")]
        public async Task<JsonResult> DoAuthenticatedEcho()
        {
            var jwt = await HttpContext.Authentication.GetAuthenticateInfoAsync("Bearer");
            return new JsonResult(jwt.Properties.Items);
        }
    }
}
