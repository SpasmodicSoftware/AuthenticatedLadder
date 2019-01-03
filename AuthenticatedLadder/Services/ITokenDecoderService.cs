using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticatedLadder.Services
{
    public interface ITokenDecoderService
    {
        JObject Decode(string token);
    }
}
