using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthenticatedLadder.DomainModels;
using Microsoft.Extensions.Options;

namespace AuthenticatedLadder.Persistence
{
    public class LadderRepository : ILadderRepository
    {
        private long _ladderLength;
        private LadderDBContext _dbContext;

        public LadderRepository(LadderDBContext dbContext, IOptions<LadderRepositorySettings> settings)
        {
            _dbContext = dbContext;
            _ladderLength = settings.Value.Length;
        }

        public Task<List<LadderEntry>> GetTopNEntries(string ladderId, string platform, long numEntries)
        {
            throw new NotImplementedException();
        }

        public Task<LadderEntry> Upsert(LadderEntry entry)
        {
            throw new NotImplementedException();
        }

        public Task<LadderEntry> GetEntryForUser(string ladderId, string platform, string username)
        {
            throw new NotImplementedException();
        }
    }
}
