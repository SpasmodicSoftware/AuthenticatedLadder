using System;
using Jose;
using Newtonsoft.Json.Linq;

namespace AuthenticatedLadder.Services.TokenDecoder
{
    public class JWTService : ITokenDecoderService
    {

        public JObject Decode(string secret, string bearerToken)
        {
            //TODO qui fare che riceviamo l'Authorization token, quindi "Bearer 123128097e8091cnbjs8d3092" e quindi splittarlo etcetc
            //così il middleware viene più semplice. E poi questa è una logica legata al JWT
            JObject result = null;
            if (!string.IsNullOrEmpty(bearerToken) && !string.IsNullOrEmpty(secret))
            {
                try
                {
                    result = JWT.Decode<JObject>(bearerToken, secret);
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
