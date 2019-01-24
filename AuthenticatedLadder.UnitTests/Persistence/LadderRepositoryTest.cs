using System;
using System.Collections.Generic;
using System.Text;
using AuthenticatedLadder.DomainModels;
using AuthenticatedLadder.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xunit;

namespace AuthenticatedLadder.UnitTests.Persistence
{
    public class LadderRepositoryTest
    {
        private LadderDBContext _dbContext;

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
        public void GetTopNEntries()
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

            _dbContext.Ladders.Add(secondPlayer);
            _dbContext.Ladders.Add(firstPlayer);
            _dbContext.Ladders.Add(anotherPlatformPlayer);
            _dbContext.SaveChanges();

            var repository = CreateInMemoryRepository(TopN);

            var result = repository.GetTopEntries(ladderId, platform);
            Assert.Single(result);

            Assert.Equal(firstPlayer, result[0]);
        }

    }
}
