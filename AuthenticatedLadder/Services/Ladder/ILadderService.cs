using AuthenticatedLadder.DomainModels;
using System.Collections.Generic;
namespace AuthenticatedLadder.Services.Ladder
{
    public interface ILadderService
    {
        List<LadderEntry> GetTopEntries(string ladderId);
        LadderEntry Upsert(LadderEntry entry);
        LadderEntry GetEntryForUser(string ladderId, string platform, string username);
        List<LadderEntry> GetAllEntriesForLadder(string ladderId);
    }
}
