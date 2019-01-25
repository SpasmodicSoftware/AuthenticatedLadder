using AuthenticatedLadder.DomainModels;
using AuthenticatedLadder.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;

namespace AuthenticatedLadder.UnitTests.Persistence
{
    public class LadderRepositoryTest
    {
        private readonly LadderDBContext _dbContext;

        private ILadderRepository CreateInMemoryRepository(int ladderLength)
        {
            return new LadderRepository(_dbContext,
                new OptionsWrapper<LadderRepositorySettings>(
                    new LadderRepositorySettings
                    {
                        Length = ladderLength
                    }));
        }

        public LadderRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<LadderDBContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;
            var dbContext = new LadderDBContext(options);
            dbContext.Database.OpenConnection();
            dbContext.Database.EnsureCreated();
            _dbContext = dbContext;
        }

        //List<LadderEntry> GetTopEntries(string ladderId, string platform);
        [Fact]
        public void GetTopEntries_ReturnsTopNEntriesForSpecifiedLadder()
        {
            var TopN = 1;
            var ladderId = "myLadder";
            var platform = "PC";
            //Here we add entries to db using context directly to test repository
            var secondPlayer = new LadderEntry
            {
                LadderId = ladderId,
                Platform = platform,
                Username = "second player",
                Score = 1000
            };
            var firstPlayer = new LadderEntry
            {
                LadderId = ladderId,
                Platform = platform,
                Username = "first player",
                Score = 999
            };
            var anotherPlatformPlayer = new LadderEntry
            {
                LadderId = ladderId,
                Platform = "AnotherPlatform",
                Username = "second player",
                Score = 1
            };
            var anotherLadderPlayer = new LadderEntry
            {
                LadderId = "AnotherLadder",
                Platform = platform,
                Username = "second player",
                Score = 1
            };

            _dbContext.Ladders.Add(secondPlayer);
            _dbContext.Ladders.Add(firstPlayer);
            _dbContext.Ladders.Add(anotherPlatformPlayer);
            _dbContext.Ladders.Add(anotherLadderPlayer);
            _dbContext.SaveChanges();

            var repository = CreateInMemoryRepository(TopN);

            var result = repository.GetTopEntries(ladderId);
            Assert.Single(result);

            Assert.Equal(anotherPlatformPlayer, result[0]);
        }

        [Fact]
        public void GetTopEntries_ReturnsTopNEntriesOrderedByScore()
        {
            var TopN = 2;
            var ladderId = "myLadder";
            var platform = "PC";
            var secondPlayer = new LadderEntry
            {
                LadderId = ladderId,
                Platform = platform,
                Username = "second player",
                Score = 1000
            };
            var firstPlayer = new LadderEntry
            {
                LadderId = ladderId,
                Platform = platform,
                Username = "first player",
                Score = 999
            };
            var anotherPlatformPlayer = new LadderEntry
            {
                LadderId = ladderId,
                Platform = "AnotherPlatform",
                Username = "second player",
                Score = 1
            };
            var anotherLadderPlayer = new LadderEntry
            {
                LadderId = "AnotherLadder",
                Platform = platform,
                Username = "second player",
                Score = 1
            };

            _dbContext.Ladders.Add(secondPlayer);
            _dbContext.Ladders.Add(firstPlayer);
            _dbContext.Ladders.Add(anotherPlatformPlayer);
            _dbContext.Ladders.Add(anotherLadderPlayer);
            _dbContext.SaveChanges();

            var repository = CreateInMemoryRepository(TopN);

            var result = repository.GetTopEntries(ladderId);

            Assert.Equal(2, result.Count);
            Assert.Equal(anotherPlatformPlayer, result[0]);
            Assert.Equal(firstPlayer, result[1]);
        }

        public void GetTopEntries_GetAllEntriesIfDBHasLessThanNEntriesForThatPlatformAndLadder() { }
        //[Fact]
        //public void GetTopEntries_() { }
        //[Fact]
        //public void GetTopEntries_() { }
        //[Fact]
        //public void GetTopEntries_() { }

        //LadderEntry Upsert(LadderEntry entry);
        [Fact]
        public void Upsert_InsertEntryIfNotAlreadyPresent() { }
        [Fact]
        public void Upsert_UpdateEntryIfAlreadyExistAndIfTheScoreIsBetter() { }
        //[Fact]
        //public void Upsert_() { }
        //[Fact]
        //public void Upsert_() { }

        //LadderEntry GetEntryForUser(string ladderId, string platform, string username);
        //[Fact]
        //public void GetEntryForUser_() { }
        //[Fact]
        //public void GetEntryForUser_() { }
        //[Fact]
        //public void GetEntryForUser_() { }
        //[Fact]
        //public void GetEntryForUser_() { }

    }
}
