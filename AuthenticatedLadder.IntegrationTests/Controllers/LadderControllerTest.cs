using AuthenticatedLadder.DomainModels;
using AuthenticatedLadder.IntegrationTests.CustomWebApplicationFactories;
using FluentAssertions;
using GenericAuthenticatedLadder;
using Jose;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AuthenticatedLadder.IntegrationTests.Controllers
{
    public class LadderControllerTest : IClassFixture<LadderCustomWebApplicationFactory<Startup>>
    {
        private readonly LadderCustomWebApplicationFactory<Startup> _factory;

        public LadderControllerTest(LadderCustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        private string PrepareJWTPayload(string payload)
        {
            return JWT.Encode(payload, _factory.JWTDecodeSecret, JweAlgorithm.PBES2_HS256_A128KW, JweEncryption.A256CBC_HS512);
        }

        [Fact]
        public async Task GetTopForLadder_ReturnsUnauthorizedIfNotValidJWTPayload()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/ladder/myLadder");

            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetTopForLadder_ReturnsEmptyListIfValidJWTPayloadButNoLadderFound()
        {
            var client = _factory.CreateClient();

            client.DefaultRequestHeaders.Add(_factory.JWTHeaderName, PrepareJWTPayload("{}"));

            var response = await client.GetAsync("/ladder/notFoundLadder");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadAsAsync<List<LadderEntry>>();

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task InsertOrUpdateEntry_ReturnsUnauthorizedIfNotValidJWTPayload()
        {
            var client = _factory.CreateClient();
            var response = await client.PostAsync("/ladder", new StringContent("{}", Encoding.UTF8, "application/json"));

            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetPlayerPosition_ReturnsUnauthorizedIfNotValidJWTPayload()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/ladder/myLadder/PC/myPlayer");

            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
