using AuthenticatedLadder.DomainModels;
using AuthenticatedLadder.Services.Ladder;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace AuthenticatedLadder.UnitTests.Services.Ladder
{
    public class LadderServiceTest
    {
        private Mock<ILadderRepository> _repository;

        public static IEnumerable<object[]> GetEntryForUserTestData => new List<object[]>
        {
            new object[]{ "", "ok", "ok"},
            new object[]{ "ok", "", "ok"},
            new object[]{ "ok", "ok", ""},
            new object[]{ null, "ok", "ok"},
            new object[]{ "ok", null, "ok"},
            new object[]{ "ok", "ok", null},
        };

        public LadderServiceTest()
        {
            _repository = new Mock<ILadderRepository>();
        }

        [Theory]
        [MemberData(nameof(GetEntryForUserTestData))]
        public void GetEntryForUser_ReturnsNullWhenInputIsNotValid(string ladderId, string platform, string username)
        {

            var service = new LadderService(_repository.Object);

            Assert.Null(service.GetEntryForUser(ladderId, platform, username));
        }

        [Fact]
        public void GetEntryForUser_ReturnsNullWhenInputIsOkButThereAreNoEntriesCorrespondingToThatKey()
        {
            var myLadderId = "My Ladder Id";
            var myPlatform = "PC";
            var myUsername = "My Username";
            _repository
                .Setup(r => r.GetEntryForUser(myLadderId, myPlatform, myUsername))
                .Returns<LadderEntry>(null);

            var service = new LadderService(_repository.Object);

            Assert.Null(service.GetEntryForUser(myLadderId, myPlatform, myUsername));

        }

        [Fact]
        public void GetEntryForUser_ReturnsMyEntryWhenMyKeyMatchesOneEntry()
        {
            var myLadderId = "My Ladder Id";
            var myPlatform = "PC";
            var myUsername = "My Username";
            var myEntry = new LadderEntry
            {
                LadderId = myLadderId,
                Platform = myPlatform,
                Username = myUsername
            };
            _repository
                .Setup(r => r.GetEntryForUser(myLadderId, myPlatform, myUsername))
                .Returns(() => myEntry);

            var service = new LadderService(_repository.Object);
            var result = service.GetEntryForUser(myLadderId, myPlatform, myUsername);

            Assert.Equal(myEntry, result);
        }

        [Fact(Skip = "TODO")]
        public void GetEntryForUser_LogsErrorAndThrowsWhenRepositoryThrows() { }


        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetTopEntries_ReturnsEmptyListIfLadderIdIsNullOrEmpty(string ladderId)
        {

            var service = new LadderService(_repository.Object);

            var result = service.GetTopEntries(ladderId);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact(Skip = "TODO")]
        public void GetTopEntries_ReturnsEmptyListIfLadderIdIsOkButServerHasNoEntryForThatLadder() { }

        [Fact(Skip = "TODO")]
        public void GetTopEntries_ReturnTopEntriesOfGivenLadder() { }

        [Fact("TODO")]
        public void GetTopEntries_LogsErrorAndThrowsWhenRepositoryThrows() { }

    }
}
