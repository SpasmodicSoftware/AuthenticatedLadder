﻿using Jose;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace AuthenticatedLadder.Services
{
    public class JWTService : ITokenDecoderService
    {
        private string _jwtSecret;

        public JWTService(string jwtSecret)
        {
            _jwtSecret = jwtSecret;
        }

        public JObject Decode(string bearerToken)
        {
            //TODO qui fare che riceviamo l'Authorization token, quindi "Bearer 123128097e8091cnbjs8d3092" e quindi splittarlo etcetc
            //così il middleware viene più semplice. E poi questa è una logica legata al JWT
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
