using AuthenticatedLadder.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GenericAuthenticatedLadder.Controllers
{
    public class AuthenticatedEchoController : ControllerBase
    {
        private ITokenDecoderService _decoderService;

        public AuthenticatedEchoController(ITokenDecoderService decoderService)
        {
            _decoderService = decoderService;
        }

        [HttpGet("echo")]
        public IActionResult DoAuthenticatedEcho()
        {
            return new JsonResult(Request.HttpContext.Items["JWTSignedPayload"]);
        }

    }
}
