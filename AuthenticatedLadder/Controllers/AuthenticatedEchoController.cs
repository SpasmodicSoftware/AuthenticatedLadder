using AuthenticatedLadder.Services.JWTPayloadHolder;
using Microsoft.AspNetCore.Mvc;

namespace GenericAuthenticatedLadder.Controllers
{
    public class AuthenticatedEchoController : ControllerBase
    {
        private IJWTPayloadHolder _payloadHolder;

        public AuthenticatedEchoController(IJWTPayloadHolder payloadHolder)
        {
            _payloadHolder = payloadHolder;
        }

        [HttpGet("echo")]
        public IActionResult DoAuthenticatedEcho()
        {
            //return new JsonResult(Request.HttpContext.Items["JWTSignedPayload"]);
            return new JsonResult(_payloadHolder.GetPayload("JWTSignedPayload"));
        }

    }
}
