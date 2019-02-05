using AuthenticatedLadder.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace AuthenticatedLadder.Services.JWTPayloadHolder
{
    public class JWTPayloadHolder : IJWTPayloadHolder
    {
        private Dictionary<string, JObject> _payloadHolder;
        private ILoggerAdapter<JWTPayloadHolder> _logger;

        public JWTPayloadHolder(ILoggerAdapter<JWTPayloadHolder> logger)
        {
            _logger = logger;
            _payloadHolder = new Dictionary<string, JObject>();
        }

        public JObject GetPayload(string payloadName)
        {
            //TODO LOG
            return _payloadHolder[payloadName];
        }

        public void HoldPayload(string payloadName, JObject payload)
        {
            //TODO LOG
            _payloadHolder[payloadName] = payload;
        }
    }
}
