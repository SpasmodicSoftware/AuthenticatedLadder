using AuthenticatedLadder.DomainModels;
using AuthenticatedLadder.Logging;
using AuthenticatedLadder.Services.JWTPayloadHolder;
using AuthenticatedLadder.Services.Ladder;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AuthenticatedLadder.Controllers
{
    [Route("ladder")]
    public class LadderController : ControllerBase
    {
        private ILadderService _ladderService;
        private ILoggerAdapter<LadderController> _logger;
        private IJWTPayloadHolder _jwtPayload;

        public LadderController(ILadderService ladderService, ILoggerAdapter<LadderController> logger, IJWTPayloadHolder jwtPayload)
        {
            _ladderService = ladderService;
            _logger = logger;
            _jwtPayload = jwtPayload;
        }

        [Route("{ladderId}"), HttpGet]
        public IEnumerable<LadderEntry> GetTopForLadder(string ladderId)
        {
            return _ladderService.GetTopEntries(ladderId);
        }

        [HttpPost]
        public ActionResult InsertOrUpdateEntry()
        {
            var entry = _jwtPayload.GetPayload("JWTSignedPayload").ToObject<LadderEntry>(); 
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

        [Route("{ladderId}/all"), HttpGet]
        public IEnumerable<LadderEntry> GetAllEntriesForLadder(string ladderId)
        {
            return _ladderService.GetAllEntriesForLadder(ladderId);
        }

        [Route("{ladderId}/{platform}/{username}"), HttpDelete]
        public ActionResult DeleteEntry(string ladderId, string platform, string username)
        {
            if(_ladderService.DeleteEntry(ladderId, platform, username))
            {
                _logger.LogInformation($"Entry <{platform},{username}> deleted from ladder <{ladderId}>");
                return Ok();
            }
            return NotFound();
        }
    }
}
