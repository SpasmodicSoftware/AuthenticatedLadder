using AuthenticatedLadder.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
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
            Assert.False(settings.IsValidConfiguration());

            settings.Length = 25;
            Assert.True(settings.IsValidConfiguration());
           
        }
    }
}
