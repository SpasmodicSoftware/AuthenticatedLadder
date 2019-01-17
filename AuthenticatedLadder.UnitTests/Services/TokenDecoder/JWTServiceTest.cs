using System.Collections.Generic;
using AuthenticatedLadder.Services.TokenDecoder;
using Jose;
using Newtonsoft.Json.Linq;
using Xunit;

namespace AuthenticatedLadder.UnitTests.Services.TokenDecoder
{
    public class JWTServiceTest
    {
        const string workingSecret = "SuperSecret";
        private JWTService _jwtService;

        public JWTServiceTest()
        {
            _jwtService = new JWTService();
        }

        [Theory]
        [InlineData("Random.Gibberish.stuff")]
        [InlineData("23409238nuc2ur0ifope")]
        [InlineData("")]
        [InlineData(null)]
        public void IfBearerTokenIsGibberishReturnsNull(string gibberishValue)
        {
            var result = _jwtService.Decode(workingSecret, gibberishValue);
            Assert.Null(result);
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

            var result = _jwtService.Decode(workingSecret, token);
            Assert.Null(result);

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
            var result = _jwtService.Decode(workingSecret, token);

            Assert.NotNull(result);
            Assert.True(JObject.DeepEquals(payload, result));
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
            Assert.Null(_jwtService.Decode(null, token));
        }
    }
}
