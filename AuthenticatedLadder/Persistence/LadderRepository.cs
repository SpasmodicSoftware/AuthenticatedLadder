﻿using System;
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
                existingEntry.Score = entry.Score;
            }
            else
            {
                _dbContext.Ladders.Add(entry);
            }
            _dbContext.SaveChanges();
            return entry;
        }

        public LadderEntry GetEntryForUser(string ladderId, string platform, string username)
        {
            return _dbContext.Ladders
                .FirstOrDefault(l => l.LadderId == ladderId
                                     && l.Platform == platform
                                     && l.Username == username);
        }
    }
}
