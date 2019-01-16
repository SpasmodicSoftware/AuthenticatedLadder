using AuthenticatedLadder.Services;
using Jose;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Xunit;

namespace AuthenticatedLadder.UnitTests.Services
{
    public class JWTServiceTest
    {
        const string workingSecret = "SuperSecret";
        private JWTService _jwtService;

        public JWTServiceTest()
        {
            _jwtService = new JWTService(workingSecret);
        }

        [Theory]
        [InlineData("Random.Gibberish.stuff")]
        [InlineData("23409238nuc2ur0ifope")]
        [InlineData(null)]
        public void IfBearerTokenIsGibberishReturnsNull(string gibberishValue)
        {
            var result = _jwtService.Decode("");
            Assert.Null(result);

            result = _jwtService.Decode(null);
            Assert.Null(result);

            result = _jwtService.Decode(gibberishValue);
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

            string token = JWT.Encode(payload, "a different secret", JweAlgorithm.PBES2_HS256_A128KW, JweEncryption.A256CBC_HS512);

            var result = _jwtService.Decode(token);
            Assert.Null(result);

        }

        [Fact]
        public void IFBearerTokenIsMadeWithCorrectSecretDecodesItAndReturnsJsonPayload()
        {
            var payload = new JObject()
            {
                { "sub", "mr.x@contoso.com" },
                { "exp", 1300819380 }
            };

            string token = JWT.Encode(payload, workingSecret, JweAlgorithm.PBES2_HS256_A128KW, JweEncryption.A256CBC_HS512);
            var result = _jwtService.Decode(token);

            Assert.NotNull(result);
            Assert.True(JObject.DeepEquals(payload, result));
        }
    }
}
