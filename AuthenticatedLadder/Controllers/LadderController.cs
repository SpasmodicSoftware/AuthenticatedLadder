using AuthenticatedLadder.DomainModels;
using AuthenticatedLadder.Logging;
using AuthenticatedLadder.Services.Ladder;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AuthenticatedLadder.Controllers
{
    [Route("ladder")]
    public class LadderController : ControllerBase
    {
        private ILadderService _ladderService;
        private ILoggerAdapter<LadderController> _logger;

        public LadderController(ILadderService ladderService, ILoggerAdapter<LadderController> logger)
        {
            _ladderService = ladderService;
            _logger = logger;
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
                _logger.LogInformation("Returning bad request for entry. Maybe malformed request?");
                return BadRequest();
            }

            _logger.LogInformation("Successfully added or updated entry");
            return Ok(result);
        }

        [Route("{ladderId}/{platform}/{username}"), HttpGet]
        public ActionResult GetPlayerPosition(string ladderId, string platform, string username)
        {
            var result = _ladderService.GetEntryForUser(ladderId, platform, username);
            if (result == null)
            {
                _logger.LogInformation($"Cannot find user <{username},{platform}> in ladder {ladderId}");
                return NotFound();
            }
            return Ok(result);
        }
    }
}
