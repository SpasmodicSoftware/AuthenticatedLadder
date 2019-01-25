using AuthenticatedLadder.DomainModels;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AuthenticatedLadder.Persistence
{
    public class LadderRepository : ILadderRepository
    {
        private int _numEntries;
        private LadderDBContext _dbContext;

        private long computeCurrentPosition(string ladderId, long score)
        {
            return _dbContext.Ladders.Count(l => l.LadderId == ladderId && l.Score < score ) + 1;
        }

        public LadderRepository(LadderDBContext dbContext, IOptions<LadderRepositorySettings> settings)
        {
            _dbContext = dbContext;
            _numEntries = settings.Value.Length;
        }

        public List<LadderEntry> GetTopEntries(string ladderId)
        {
            var result = _dbContext.Ladders
                .Where(l => l.LadderId == ladderId)
                .OrderBy(l => l.Score)
                .Take(_numEntries)
                .ToList();
            var i = 0;
            result.ForEach(l => l.Position = (i++) + 1);
            return result;
        }

        public LadderEntry Upsert(LadderEntry entry)
        {
            var resultingEntry = _dbContext.Ladders
                .Where(l => l.LadderId == entry.LadderId
                            && l.Platform == entry.Platform
                            && l.Username == entry.Username)
                .FirstOrDefault();
            if (resultingEntry != null)
            {
                resultingEntry.Score = resultingEntry.Score < entry.Score
                    ? resultingEntry.Score : entry.Score;
            }
            else
            {
                _dbContext.Ladders.Add(entry);
                resultingEntry = entry;
            }
            _dbContext.SaveChanges();

            resultingEntry.Position = computeCurrentPosition(resultingEntry.LadderId, resultingEntry.Score);
            return resultingEntry;
        }

        public LadderEntry GetEntryForUser(string ladderId, string platform, string username)
        {
            var result = _dbContext.Ladders
                .FirstOrDefault(l => l.LadderId == ladderId
                                     && l.Platform == platform
                                     && l.Username == username);
            if (result != null)
            {
                result.Position = computeCurrentPosition(result.LadderId, result.Score);
            }

            return result;
        }
    }
}
