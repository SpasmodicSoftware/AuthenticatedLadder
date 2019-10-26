using AuthenticatedLadder.DomainModels;
using AuthenticatedLadder.Logging;
using AuthenticatedLadder.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AuthenticatedLadder.UnitTests.Persistence
{
    public class LadderRepositoryTest
    {
        private readonly LadderDBContext _dbContext;
        private readonly Mock<ILoggerAdapter<LadderRepository>> _logger;

        private ILadderRepository CreateInMemoryRepository(int ladderLength)
        {
            return new LadderRepository(_dbContext,
                new OptionsWrapper<LadderRepositorySettings>(
                    new LadderRepositorySettings
                    {
                        Length = ladderLength
                    }),
                _logger.Object);
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

            _logger = new Mock<ILoggerAdapter<LadderRepository>>();
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

            repository
                .GetTopEntries(ladderId)
                .Should()
                .HaveCount(1)
                .And.ContainEquivalentOf(anotherPlatformPlayer);

            _logger.Verify(l => l.LogInformation(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void GetTopEntries_ReturnsTopNEntriesOrderedByScoreTheLessTheBetter()
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

            repository
                .GetTopEntries(ladderId)
                .Should()
                .HaveCount(2)
                .And.HaveElementAt(0, anotherPlatformPlayer)
                .And.HaveElementAt(1, firstPlayer);

            _logger.Verify(l => l.LogInformation(It.IsAny<string>()), Times.Once());
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

            repository
                .GetTopEntries(ladderId)
                .Should()
                .HaveCount(1)
                .And.ContainEquivalentOf(secondPlayer);

            _logger.Verify(l => l.LogInformation(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void GetTopEntries_PopulatesPlayerPosition()
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
                Score = 999,
                Position = 2
            };
            var anotherPlatformPlayer = new LadderEntry
            {
                LadderId = ladderId,
                Platform = "AnotherPlatform",
                Username = "second player",
                Score = 1,
                Position = 1
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

            repository
                .GetTopEntries(ladderId)
                .Should()
                .HaveCount(2)
                .And.HaveElementAt(0, anotherPlatformPlayer)
                .And.HaveElementAt(1, firstPlayer);

            _logger.Verify(l => l.LogInformation(It.IsAny<string>()), Times.Once());
        }
        [Fact]
        public void Upsert_SuccessfullyInsertNewEntriesRegardingTheTopPlayerLadderSize()
        {
            var TopPlayerLadderSize = 2;
            var ladderId = "myLadder";
            var platform = "PC";
            var usernames = new[] { "second player", "first player", "second player" };
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

            _dbContext
                .Ladders
                .Should()
                .HaveCount(1);

            //Position is not relevant right now
            result.Position = 0;
            result
                .Should()
                .BeEquivalentTo(firstEntry);

            result = repository.Upsert(thirdEntryBetterScore);

            _dbContext
                .Ladders
                .Should()
                .HaveCount(1);

            //Position is not relevant right now
            result.Position = 0;
            result
                .Should()
                .BeEquivalentTo(thirdEntryBetterScore);

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

            _dbContext
                .Ladders
                .Should()
                .HaveCount(2);

        }
        [Fact]
        public void Upsert_PopulatesPlayerPosition()
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
                Score = 3000,
                Position = 2
            };

            var repository = CreateInMemoryRepository(topN);
            repository.Upsert(firstEntry);

            repository
                .Upsert(secondEntryDifferentPlatform)
                .Should()
                .BeEquivalentTo(secondEntryDifferentPlatform);

            _dbContext
                .Ladders
                .Should()
                .HaveCount(2);

        }

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
                Score = 500,
                Position = 1
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

            repository
                .GetEntryForUser(ladderId, "Another Platform", playerName)
                .Should()
                .NotBeNull()
                .And.BeEquivalentTo(thirdEntry);
        }

        [Fact]
        public void GetAllEntriesForPlatform_ReturnsAllEntriesForSpecifiedLadder()
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
                Platform = "AnotherPlatform",
                Username = "second player",
                Score = 1
            };

            _dbContext.Ladders.Add(secondPlayer);
            _dbContext.Ladders.Add(firstPlayer);
            _dbContext.Ladders.Add(anotherPlatformPlayer);
            _dbContext.Ladders.Add(anotherLadderPlayer);

            _dbContext.SaveChanges();

            var repository = CreateInMemoryRepository(TopN);

            repository
                .GetAllEntriesForLadder(ladderId)
                .Should()
                .HaveCount(3)
                .And.ContainEquivalentOf(anotherPlatformPlayer)
                .And.ContainEquivalentOf(firstPlayer)
                .And.ContainEquivalentOf(secondPlayer);
        }

        [Fact]
        public void DeleteEntry_ReturnsFalseWhenDatabaseIsEmpty()
        {
            var repository = CreateInMemoryRepository(1);

            repository.DeleteEntry("foo", "bar", "baz")
                .Should()
                .BeFalse();
        }

        [Fact]
        public void DeleteEntry_ReturnsFalseWhenGivenEntryIsNotInTheDatabaseAndDoesntRemoveTheEntry()
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
                Platform = "AnotherPlatform",
                Username = "second player",
                Score = 1
            };

            _dbContext.Ladders.Add(secondPlayer);
            _dbContext.Ladders.Add(firstPlayer);
            _dbContext.Ladders.Add(anotherPlatformPlayer);
            _dbContext.Ladders.Add(anotherLadderPlayer);

            _dbContext.SaveChanges();

            var repository = CreateInMemoryRepository(TopN);

            repository.DeleteEntry("foo", "bar", "baz")
               .Should()
               .BeFalse();

            _dbContext.Ladders.Count()
                .Should().Be(4);
        }

        [Fact]
        public void DeleteEntry_ReturnsTrueWhenGivenEntryIsInTheDatabaseAndRemovesTheEntry()
        {
            var TopN = 1;
            var ladderId = "myLadder";
            var platform = "PC";
            var playerName = "Giampaolo";
            //Here we add entries to db using context directly to test repository
            var secondPlayer = new LadderEntry
            {
                LadderId = ladderId,
                Platform = platform,
                Username = playerName,
                Score = 1000
            };
            _dbContext.Ladders.Add(secondPlayer);
            _dbContext.SaveChanges();

            var repository = CreateInMemoryRepository(TopN);

            repository.DeleteEntry(ladderId, platform, playerName)
              .Should()
              .BeTrue();

            _dbContext.Ladders.Count()
                .Should().Be(0);
        }

    }
}
