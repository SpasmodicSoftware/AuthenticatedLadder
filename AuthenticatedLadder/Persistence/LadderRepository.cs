using AuthenticatedLadder.DomainModels;
using AuthenticatedLadder.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace AuthenticatedLadder.Persistence
{
    public class LadderRepository : ILadderRepository
    {
        private readonly int _numEntries;
        private ILoggerAdapter<LadderRepository> _logger;
        private LadderDBContext _dbContext;

        private long ComputeCurrentPosition(string ladderId, long score)
        {
            return _dbContext.Ladders.Count(l => l.LadderId == ladderId && l.Score < score) + 1;
        }

        public LadderRepository(LadderDBContext dbContext, IOptions<LadderRepositorySettings> settings, ILoggerAdapter<LadderRepository> logger)
        {
            _dbContext = dbContext;
            _numEntries = settings.Value.Length;
            _logger = logger;
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

            _logger.LogInformation($"Got {result.Count} position for ladder {ladderId}");

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
                _logger.LogInformation("<{0},{1},{2}> already exist. Updating.", entry.LadderId, entry.Platform, entry.Username);

                resultingEntry.Score = resultingEntry.Score < entry.Score
                    ? resultingEntry.Score : entry.Score;
            }
            else
            {
                _logger.LogInformation("<{0},{1},{2}> needs to be created.", entry.LadderId, entry.Platform, entry.Username);
                _dbContext.Ladders.Add(entry);
                resultingEntry = entry;
            }
            _dbContext.SaveChanges();

            resultingEntry.Position = ComputeCurrentPosition(resultingEntry.LadderId, resultingEntry.Score);

            _logger.LogInformation("Current position in ladder {0} is {1}", resultingEntry.LadderId, resultingEntry.Position);

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
                _logger.LogInformation("Entry for <{0},{1},{2}> found", ladderId, platform, username);

                result.Position = ComputeCurrentPosition(result.LadderId, result.Score);

                _logger.LogInformation("Position is {0}", result.Position);
            }

            return result;
        }

        public List<LadderEntry> GetAllEntriesForPlatform(string ladderId)
        {
            var result = _dbContext.Ladders
                .Where(l => l.LadderId == ladderId)
                .ToList();
            return result;
        }
    }
}
