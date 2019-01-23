using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticatedLadder.DomainModels
{
    public interface ILadderRepository
    {
        Task<List<LadderEntry>> GetTopNEntries(string ladderId, string platform, long numEntries);
        Task<LadderEntry> Upsert(LadderEntry entry);
        Task<LadderEntry> GetEntryForUser(string ladderId, string platform, string username);
    }
}