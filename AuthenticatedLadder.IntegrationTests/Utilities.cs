using AuthenticatedLadder.DomainModels;
using AuthenticatedLadder.Persistence;
using System.Collections.Generic;

namespace AuthenticatedLadder.IntegrationTests
{
    public static class Utilities
    {
        public static void PrepareDatabaseForTest(LadderDBContext db)
        {
            db.Ladders.Add(new LadderEntry()
            {
                LadderId = "A Ldder",
                Platform = "Nintendo360",
                Username = "Antani",
                Score = 666
            });
            db.SaveChanges();

        }

        private static readonly List<LadderEntry> __;
    }
}
