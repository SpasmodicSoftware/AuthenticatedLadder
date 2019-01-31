using AuthenticatedLadder.Persistence;
using FluentAssertions;
using Xunit;

namespace AuthenticatedLadder.UnitTests.Persistence
{
    public class LadderRepositorySettingsTest
    {
        [Fact]
        public void ASettingsIsValidWhenLengthIsMoreThanZero()
        {
            var settings = new LadderRepositorySettings();

            settings.Length = 0;
            settings
                .IsValidConfiguration()
                .Should()
                .BeFalse();

            settings.Length = 25;
            settings
                .IsValidConfiguration()
                .Should()
                .BeTrue();

        }
    }
}
