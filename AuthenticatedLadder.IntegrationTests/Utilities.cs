using AuthenticatedLadder.DomainModels;
using AuthenticatedLadder.Persistence;
using System;
using System.Collections.Generic;

namespace AuthenticatedLadder.IntegrationTests
{
    public static class Utilities
    {
        public static void PrepareDatabaseForTest(LadderDBContext db)
        {
            db.Ladders.AddRange();
        }

        private static readonly List<LadderEntry> __;
    }
}
