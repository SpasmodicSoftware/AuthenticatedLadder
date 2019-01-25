using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticatedLadder.DomainModels;
using Microsoft.Extensions.Options;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace AuthenticatedLadder.Persistence
{
    public class LadderRepository : ILadderRepository
    {
        private int _numEntries;
        private LadderDBContext _dbContext;

        public LadderRepository(LadderDBContext dbContext, IOptions<LadderRepositorySettings> settings)
        {
            _dbContext = dbContext;
            _numEntries = settings.Value.Length;
        }

        public List<LadderEntry> GetTopEntries(string ladderId)
        {
            //TODO use a config flag in order to identify which orderby use
            //this is usefult if score is number of point or if it is
            //seconds elapsed from game start
            return _dbContext.Ladders
                .Where(l => l.LadderId == ladderId)
                .OrderBy(l => l.Score)
                .Take(_numEntries)
                .ToList();
        }

        public LadderEntry Upsert(LadderEntry entry)
        {
            var existingEntry = _dbContext.Ladders
                .Where(l => l.LadderId == entry.LadderId 
                            && l.Platform == entry.Platform 
                            && l.Username == entry.Username)
                .FirstOrDefault();
            if (existingEntry != null)
            {
                existingEntry.Score = existingEntry.Score < entry.Score 
                    ? existingEntry.Score : entry.Score;
            }
            else
            {
                _dbContext.Ladders.Add(entry);
            }
            _dbContext.SaveChanges();
            return existingEntry ?? entry;
        }

        public LadderEntry GetEntryForUser(string ladderId, string platform, string username)
        {
            var result =_dbContext.Ladders
                .FirstOrDefault(l => l.LadderId == ladderId
                                     && l.Platform == platform
                                     && l.Username == username);
            result.Position = _dbContext.Ladders.Count(l => l.Score < result.Score);
            return result;
        }
    }
}
