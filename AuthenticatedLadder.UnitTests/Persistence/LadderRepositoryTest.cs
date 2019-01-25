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
        public void Upsert_UpdateEntryIfAlreadyExistAndOnlyIfTheScoreIsBetter()
        {
            var topN = 5;
            var ladderId = "myLadder";
            var platform = "PC";
            var playerName = "My Player";
            var firstEntry = new LadderEntry
            {
                LadderId = ladderId,
                Platform = platform,
                Username = playerName,
                Score = 1000
            };
            var secondEntryWorstScore = new LadderEntry
            {
                LadderId = ladderId,
                Platform = platform,
                Username = playerName,
                Score = 3000
            };
            var thirdEntryBetterScore = new LadderEntry
            {
                LadderId = ladderId,
                Platform = platform,
                Username = playerName,
                Score = 900
            };

            var repository = CreateInMemoryRepository(topN);

            var result = repository.Upsert(firstEntry);
            repository.Upsert(secondEntryWorstScore);

            Assert.Single(_dbContext.Ladders);
            Assert.Equal(firstEntry, result);

            result = repository.Upsert(thirdEntryBetterScore);

            Assert.Single(_dbContext.Ladders);
            Assert.Equal(thirdEntryBetterScore, result);
        }

        [Fact]
        public void Upsert_UsernameIsUniquePerPlatformAndPerLadderId()
        {
            var topN = 5;
            var ladderId = "myLadder";
            var playerName = "My Player";
            var firstEntry = new LadderEntry
            {
                LadderId = ladderId,
                Platform = "PC",
                Username = playerName,
                Score = 1000
            };
            var secondEntryDifferentPlatform = new LadderEntry
            {
                LadderId = ladderId,
                Platform = "PS4",
                Username = playerName,
                Score = 3000
            };

            var repository = CreateInMemoryRepository(topN);
            repository.Upsert(firstEntry);
            repository.Upsert(secondEntryDifferentPlatform);

            Assert.Equal(2, _dbContext.Ladders.Count());

        }

        //LadderEntry GetEntryForUser(string ladderId, string platform, string username);
        [Fact]
        public void GetEntryForUser_ReturnsNullIfUserNotPresent()
        {
            var topN = 5;
            var ladderId = "myLadder";
            var playerName = "My Player";
            var firstEntry = new LadderEntry
            {
                LadderId = ladderId,
                Platform = "PC",
                Username = playerName,
                Score = 1000
            };

            _dbContext.Ladders.Add(firstEntry);
            _dbContext.SaveChanges();

            var repository = CreateInMemoryRepository(topN);

            var result = repository.GetEntryForUser(ladderId, "AnotherPlatform", playerName);
            Assert.Null(result);
        }
        [Fact]
        public void GetEntryForUser_ComputesThePositionOfTheUserInTheLadder()
        {
            var topN = 5;
            var ladderId = "myLadder";
            var platform = "PC";
            var playerName = "My Player";
            var firstEntry = new LadderEntry
            {
                LadderId = ladderId,
                Platform = platform,
                Username = playerName,
                Score = 1001
            };            
            var secondEntry = new LadderEntry
            {
                LadderId = ladderId,
                Platform = platform,
                Username = "Second Player",
                Score = 1002
            };
            var thirdEntry = new LadderEntry
            {
                LadderId = ladderId,
                Platform = "Another Platform",
                Username = playerName,
                Score = 500
            };            
            var fourthEntry = new LadderEntry
            {
                LadderId = "Another Ladder",
                Platform = "Another Platform",
                Username = playerName,
                Score = 500
            };

            _dbContext.Ladders.Add(firstEntry);
            _dbContext.Ladders.Add(secondEntry);
            _dbContext.Ladders.Add(thirdEntry);
            _dbContext.Ladders.Add(fourthEntry);
            _dbContext.SaveChanges();

            var repository = CreateInMemoryRepository(topN);

            var result = repository.GetEntryForUser(ladderId, "Another Platform", playerName);
            Assert.NotNull(result);
            Assert.Equal(thirdEntry, result);
            Assert.Equal(1,result.Position);
        }
        //[Fact]
        //public void GetEntryForUser_() { }

    }
}
