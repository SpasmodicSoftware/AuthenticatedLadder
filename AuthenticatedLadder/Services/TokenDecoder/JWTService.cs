using System;
using Jose;
using Newtonsoft.Json.Linq;

namespace AuthenticatedLadder.Services.TokenDecoder
{
    public class JWTService : ITokenDecoderService
    {

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
                    //TODO LOG error, invalid keyword
                }
                catch (ArgumentOutOfRangeException)
                {
                    //TODO log error invalid token
                }
                catch (IndexOutOfRangeException)
                {
                    //TODO log the fact that someone is trying to pass us some random crap
                }
            }

            return result;
        }

    }
}
