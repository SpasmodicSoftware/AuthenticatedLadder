using AuthenticatedLadder.DomainModels;
using AuthenticatedLadder.Persistence;
using System.Collections.Generic;

namespace AuthenticatedLadder.IntegrationTests
{
    public static class Utilities
    {
        public static void PrepareDatabaseForTest(LadderDBContext db)
        {
            db.Ladders.AddRange(
                new LadderEntry()
                {
                    LadderId = "existingLadder",
                    Platform = "Nintendo360",
                    Username = "Antani",
                    Score = 666
                },
                new LadderEntry()
                {
                    LadderId = "existingLadder",
                    Platform = "Super Famicom",
                    Username = "secondo",
                    Score = 6666
                },new LadderEntry()
                {
                    LadderId = "existingLadder",
                    Platform = "Nintendo360",
                    Username = "Ultimo",
                    Score = 999999
                }
            );
            db.SaveChanges();

        }

        private static readonly List<LadderEntry> __;
    }
}
