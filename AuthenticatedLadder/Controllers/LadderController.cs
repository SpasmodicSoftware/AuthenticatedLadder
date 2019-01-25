using AuthenticatedLadder.DomainModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace AuthenticatedLadder.Controllers
{
    [Route("ladder")]
    public class LadderController : ControllerBase
    {
        public LadderController()
        {

        }

        [Route("{ladderId}"), HttpGet]
        public IEnumerable<LadderEntry> GetTopForLadder(long ladderId)
        {
            throw new NotImplementedException();
        }

        [Route("{ladderId}"), HttpPost]
        public HttpResponseMessage InsertOrUpdateEntry(long ladderId, LadderEntry entry)
        {
            throw new NotImplementedException();
        }

        [Route("{ladderId}/{username}"), HttpGet]
        public LadderEntry GetPlayerPosition(string ladderId, string platform, string username)
        {
            throw new NotImplementedException();
        }
    }
}
