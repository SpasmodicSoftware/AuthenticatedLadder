using Jose;
using Newtonsoft.Json.Linq;
using System;

namespace AuthenticatedLadder.Services
{
    public class JWTService
    {
        private string _jwtSecret;

        public JWTService(string jwtSecret)
        {
            _jwtSecret = jwtSecret;
        }

        public JObject Decode(String bearerToken)
        {
            JObject result = null;
            if (!string.IsNullOrEmpty(bearerToken))
            {
                try
                {
                    result = JWT.Decode<JObject>(bearerToken, _jwtSecret);
                }
                catch (IntegrityException)
                {
                    //TODO LOG error, invalid keyword
                }
                catch (ArgumentOutOfRangeException)
                {
                    //TODO log error invalid token
                }
            }

            return result;
        }

    }
}
