using Newtonsoft.Json.Linq;

namespace AuthenticatedLadder.Services.JWTPayloadHolder
{
    public interface IJWTPayloadHolder
    {
        void HoldPayload(string payloadName, JObject payload);
        JObject GetPayload(string payloadName);
    }
}
