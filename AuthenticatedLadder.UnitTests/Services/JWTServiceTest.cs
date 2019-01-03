using AuthenticatedLadder.Services;
using Jose;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Xunit;

namespace AuthenticatedLadder.UnitTests.Services
{
    public class JWTServiceTest
    {
        const string _superSecret = "SuperSecret";
        private JWTService _jwtService;

        public JWTServiceTest()
        {
            _jwtService = new JWTService(_superSecret);
        }

        [Fact]
        public void IfBearerTokenIsGibberishReturnsNull()
        {
            var result = _jwtService.Decode("");
            Assert.Null(result);

            result = _jwtService.Decode(null);
            Assert.Null(result);

            result = _jwtService.Decode("Random.Gibberish.stuff");
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

            string token = JWT.Encode(payload, "top secret", JweAlgorithm.PBES2_HS256_A128KW, JweEncryption.A256CBC_HS512);

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

            string token = JWT.Encode(payload, _superSecret, JweAlgorithm.PBES2_HS256_A128KW, JweEncryption.A256CBC_HS512);
            var result = _jwtService.Decode(token);

            Assert.NotNull(result);
            Assert.True(JObject.DeepEquals(payload, result));
        }
    }
}
