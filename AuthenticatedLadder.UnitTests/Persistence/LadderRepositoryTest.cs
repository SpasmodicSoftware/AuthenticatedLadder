using System;
using System.Collections.Generic;
using System.Linq;
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

        [Fact]
        public void GetTopEntries_GetAllEntriesIfDBHasLessThanNEntriesForThatLadder()
        {
            var TopN = 10;
            var ladderId = "myLadder";
            var platform = "PC";
            var secondPlayer = new LadderEntry
            {
                LadderId = ladderId,
                Platform = platform,
                Username = "second player",
                Score = 1000
            };

            _dbContext.Ladders.Add(secondPlayer);
            _dbContext.SaveChanges();

            var repository = CreateInMemoryRepository(TopN);

            var result = repository.GetTopEntries(ladderId);

            Assert.Single(result);
            Assert.Equal(secondPlayer, result[0]);
        }

        //LadderEntry Upsert(LadderEntry entry);
        [Fact]
        public void Upsert_SuccessfullyInsertNewEntriesRegardingTheTopPlayerLadderSize()
        {
            var TopPlayerLadderSize = 2;
            var ladderId = "myLadder";
            var platform = "PC";
            var usernames = new[] {"second player", "first player", "second player"};
            var players = new List<LadderEntry>
            {
                new LadderEntry
                {
                    LadderId = ladderId,
                    Platform = platform,
                    Username = usernames[0],
                    Score = 1000
                },
                new LadderEntry
                {
                    LadderId = ladderId,
                    Platform = platform,
                    Username = usernames[1],
                    Score = 999
                },
                new LadderEntry
                {
                    LadderId = ladderId,
                    Platform = "AnotherPlatform",
                    Username = usernames[2],
                    Score = 1
                }
            };

            var repository = CreateInMemoryRepository(TopPlayerLadderSize);

            foreach (var ladderEntry in players)
            {
                var result = repository.Upsert(ladderEntry);
                Assert.NotNull(result);
            }

            var dbEntries = _dbContext.Ladders
                .Where(l => true)
                .OrderByDescending(l => l.Score)
                .ToList();
            Assert.Equal(3, dbEntries.Count);

            for (var i = 0; i < 3; ++i)
            {
                Assert.Equal(dbEntries[i], players[i]);
            }
        }
        [Fact]
        public void Upsert_UpdateEntryIfAlreadyExistAndIfTheScoreIsBetter() { }
        [Fact]
        public void Upsert_UsernameIsUniquePerPlatformAndPerLadderId() { }
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
