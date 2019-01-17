using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticatedLadder.Middlewares
{
    public class JWTPayloadMiddlewareSettings
    {
        public string HeaderName { get; set; }
        public string DecodeSecret { get; set; }

        public bool isValidConfiguration()
        {
            return !string.IsNullOrEmpty(HeaderName) 
                   && !string.IsNullOrEmpty(DecodeSecret);
        }
    }
}
