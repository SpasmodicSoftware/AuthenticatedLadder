using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticatedLadder.DomainModels
{
    public interface ILadderRepository
    {
        List<LadderEntry> GetTopEntries(string ladderId);
        LadderEntry Upsert(LadderEntry entry);
        LadderEntry GetEntryForUser(string ladderId, string platform, string username);
    }
}