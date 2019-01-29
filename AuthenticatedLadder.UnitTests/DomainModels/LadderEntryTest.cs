using AuthenticatedLadder.DomainModels;
using System.Collections.Generic;
using Xunit;

namespace AuthenticatedLadder.UnitTests.DomainModels
{
    public class LadderEntryTest
    {
        [Fact]
        public void TwoEntriesWithSameValuesAreEqualRegardingPositionValue()
        {
            var ladderId = "Ladder";
            var platform = "PC";
            var username = "My User";
            var score = 100;
            var entry1 = new LadderEntry()
            {
                LadderId = ladderId,
                Platform = platform,
                Score = score,
                Username = username,
                Position = 1
            };
            var entry2 = new LadderEntry()
            {
                LadderId = ladderId,
                Platform = platform,
                Username = username,
                Score = score,
                Position = 1
            };

            Assert.Equal(entry1, entry2);

            entry2.Position = 99;

            Assert.Equal(entry1, entry2);

        }

        [Fact]
        public void EntryAndNullAreNotEqual()
        {
            var entry = new LadderEntry();
            Assert.False(entry.Equals(null));
        }

        [Fact]
        public void EqualEntriesHaveSameHashCode()
        {
            var ladderId = "Ladder";
            var platform = "PC";
            var username = "My User";
            var score = 100;
            var entry1 = new LadderEntry()
            {
                LadderId = ladderId,
                Platform = platform,
                Score = score,
                Username = username,
                Position = 1
            };
            var entry2 = new LadderEntry()
            {
                LadderId = ladderId,
                Platform = platform,
                Score = score,
                Username = username,
                Position = 9
            };

            Assert.Equal(entry1.GetHashCode(), entry2.GetHashCode());
        }

        [Fact]
        public void EntriesCanBeComparedOnlyWithOtherEntries()
        {
            Assert.False(new LadderEntry().Equals("string"));
            Assert.False(new LadderEntry().Equals(new List<string>()));
        }

    }
}
