using System;
using System.Collections.Generic;
using AuthenticatedLadder.DomainModels;

namespace AuthenticatedLadder.Services.Ladder
{
    public class LadderService : ILadderService
    {

        public LadderService()
        {
            //TODO Injectare repository e logger
        }

        public LadderEntry GetEntryForUser(string ladderId, string platform, string username)
        {
            //TODO Implement me
            throw new NotImplementedException();
        }

        public List<LadderEntry> GetTopEntries(string ladderId)
        {
            //TODO Implement me
            throw new NotImplementedException();
        }

        public LadderEntry Upsert(LadderEntry entry)
        {
            //TODO Implement me
            throw new NotImplementedException();
        }
    }
}
