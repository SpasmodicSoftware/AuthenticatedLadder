using AuthenticatedLadder.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        public async Task<IActionResult> DoAuthenticatedEcho()
        {
            var payload = _decoderService.Decode(GetBearerToken());

            if (payload == null)
                return Unauthorized();

            return new JsonResult(payload);
        }

        private string GetBearerToken()
        {
            var authorizationHeaderContent = Request.Headers["Authorization"].ToString().Split(' ');
            return authorizationHeaderContent.Length == 2 ? authorizationHeaderContent[1] : null;
        }
    }
}
