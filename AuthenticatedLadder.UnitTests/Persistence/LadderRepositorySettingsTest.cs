using AuthenticatedLadder.Persistence;
using FluentAssertions;
using Xunit;

namespace AuthenticatedLadder.UnitTests.Persistence
{
    public class LadderRepositorySettingsTest
    {
        [Theory]
        [InlineData(0, false)]
        [InlineData(25, true)]
        public void ASettingsIsValidWhenLengthIsMoreThanZero(int length, bool expected)
        {
            var settings = new LadderRepositorySettings();

            settings.Length = length;
            settings
                .IsValidConfiguration()
                .Should()
                .Be(expected);
        }
    }
}
