using AuthenticatedLadder.Logging;
using Jose;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;

namespace AuthenticatedLadder.Services.TokenDecoder
{
    public class JWTService : ITokenDecoderService
    {
        private ILoggerAdapter<JWTService> _logger;

        public JWTService(ILoggerAdapter<JWTService> logger)
        {
            _logger = logger;
        }

        public JObject Decode(string secret, string token)
        {
            JObject result = null;
            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(secret))
            {
                try
                {
                    result = JWT.Decode<JObject>(token, secret);
                }
                catch (IntegrityException)
                {
                    _logger.LogWarning("Integrity exception on passed token. Token is invalid");
                }
                catch (ArgumentOutOfRangeException)
                {
                    _logger.LogWarning("ArgumentOutOfRange exception on passed token. Token is invalid");
                }
                catch (IndexOutOfRangeException)
                {
                    _logger.LogWarning("IndexOutOfRange exception on passed token. Token is invalid");
                }
            }

            return result;
        }

    }
}
