using Newtonsoft.Json.Linq;

namespace AuthenticatedLadder.Services.TokenDecoder
{
    public interface ITokenDecoderService
    {
        JObject Decode(string token);
    }
}
