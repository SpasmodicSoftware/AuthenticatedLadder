using System;
using System.Collections.Generic;
using AuthenticatedLadder.DomainModels;
namespace AuthenticatedLadder.Services.Ladder
{
    public interface ILadderService
    {
        List<LadderEntry> GetTopEntries(string ladderId);
        LadderEntry Upsert(LadderEntry entry);
        LadderEntry GetEntryForUser(string ladderId, string platform, string username);
    }
}
