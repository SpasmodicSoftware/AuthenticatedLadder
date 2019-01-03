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
        [HttpGet("echo")]
        public async Task<JsonResult> DoAuthenticatedEcho()
        {
            var jwt = Request.Headers["Authorization"];
            //TODO se jwt isNullOrEmpty return 401
            //TODO splittare Bearer 123.123.123 con ' '
            //TODO decodare jwt, se non decoda 401
            return new JsonResult(jwt);
        }
    }
}
