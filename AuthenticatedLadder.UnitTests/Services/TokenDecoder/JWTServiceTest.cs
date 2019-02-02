using AuthenticatedLadder.Services.TokenDecoder;
using FluentAssertions;
using Jose;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Xunit;

namespace AuthenticatedLadder.UnitTests.Services.TokenDecoder
{
    public class JWTServiceTest
    {
        const string workingSecret = "SuperSecret";
        private Mock<ILogger<JWTService>> _logger;

        public JWTServiceTest()
        {
            _logger = new Mock<ILogger<JWTService>>();
        }

        [Theory]
        [InlineData("Random.Gibberish.stuff")]
        [InlineData("23409238nuc2ur0ifope")]
        [InlineData("")]
        [InlineData(null)]
        public void IfBearerTokenIsGibberishReturnsNull(string gibberishValue)
        {
            new JWTService(_logger.Object)
                .Decode(workingSecret, gibberishValue)
                .Should()
                .BeNull();
        }

        [Fact]
        public void IfBearerTokenIsMadeWithAnotherSecretFailsToDecode()
        {
            var payload = new Dictionary<string, object>()
            {
                { "sub", "mr.x@contoso.com" },
                { "exp", 1300819380 }
            };

            var token = JWT.Encode(payload, "a different secret", JweAlgorithm.PBES2_HS256_A128KW, JweEncryption.A256CBC_HS512);

            new JWTService(_logger.Object)
                .Decode(workingSecret, token)
                .Should()
                .BeNull();
        }

        [Fact]
        public void IfBearerTokenIsMadeWithCorrectSecretDecodesItAndReturnsJsonPayload()
        {
            var payload = new JObject()
            {
                { "sub", "mr.x@contoso.com" },
                { "exp", 1300819380 }
            };

            var token = JWT.Encode(payload, workingSecret, JweAlgorithm.PBES2_HS256_A128KW, JweEncryption.A256CBC_HS512);

            new JWTService(_logger.Object)
                .Decode(workingSecret, token)
                .Should()
                .BeEquivalentTo(payload);
        }

        [Fact]
        public void IfSecretIsNullReturnsNull()
        {
            var payload = new JObject()
            {
                { "sub", "mr.x@contoso.com" },
                { "exp", 1300819380 }
            };

            var token = JWT.Encode(payload, workingSecret, JweAlgorithm.PBES2_HS256_A128KW, JweEncryption.A256CBC_HS512);

            new JWTService(_logger.Object)
                .Decode(null, token)
                .Should()
                .BeNull();
        }
    }
}
