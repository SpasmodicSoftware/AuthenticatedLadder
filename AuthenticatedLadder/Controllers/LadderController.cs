using AuthenticatedLadder.DomainModels;
using AuthenticatedLadder.Services.Ladder;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AuthenticatedLadder.Controllers
{
    [Route("ladder")]
    public class LadderController : ControllerBase
    {
        private ILadderService _ladderService;

        public LadderController(ILadderService ladderService)
        {
            //TODO Injectare Logger
            _ladderService = ladderService;
        }

        [Route("{ladderId}"), HttpGet]
        public IEnumerable<LadderEntry> GetTopForLadder(string ladderId)
        {
            return _ladderService.GetTopEntries(ladderId);
        }

        [HttpPost]
        public ActionResult InsertOrUpdateEntry([FromBody]LadderEntry entry)
        {
            var result = _ladderService.Upsert(entry);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [Route("{ladderId}/{platform}/{username}"), HttpGet]
        public ActionResult GetPlayerPosition(string ladderId, string platform, string username)
        {
            var result = _ladderService.GetEntryForUser(ladderId, platform, username);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
