using AuthenticatedLadder.DomainModels;
using AuthenticatedLadder.IntegrationTests.CustomWebApplicationFactories;
using FluentAssertions;
using GenericAuthenticatedLadder;
using Jose;
using Newtonsoft.Json;
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

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task InsertOrUpdateEntry_ReturnsBadRequestIfJWTPayloadIsNotAValidEntry()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(_factory.JWTHeaderName, PrepareJWTPayload("{\"another\":-1}"));

            var response = await client.PostAsync("/ladder", new StringContent("{}", Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task InsertOrUpdateEntry_ReturnsOkAndGivenEntryIfJWTPayloadIsAValidEntry()
        {
            var entry = new LadderEntry
            {
                LadderId = "My Ladder Id",
                Platform = "MyCoolPlatform",
                Username = "Ciccio42",
                Score = 12000,
                Position = 1
            };

            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(_factory.JWTHeaderName, PrepareJWTPayload(JsonConvert.SerializeObject(entry)));

            var response = await client.PostAsync("/ladder", new StringContent("{}", Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = JsonConvert.DeserializeObject<LadderEntry>(await response.Content.ReadAsStringAsync());

            result.Should().BeEquivalentTo(entry);
        }


        [Fact]
        public async Task GetPlayerPosition_ReturnsUnauthorizedIfNotValidJWTPayload()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/ladder/myLadder/PC/myPlayer");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetPlayerPosition_ReturnsNotFoundIfGivenPlayerDoesNotExistAndJWTPayloadIsSigned()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(_factory.JWTHeaderName, PrepareJWTPayload("{}"));

            var response = await client.GetAsync("/ladder/myLadder/PC/myPlayer");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetPlayerPosition_ReturnsEntryWithValuedPositionIfPlayerExistAndJWTPayloadIsSigned()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(_factory.JWTHeaderName, PrepareJWTPayload("{}"));

            var response = await client.GetAsync("/ladder/existingLadder/Nintendo360/Ultimo");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = JsonConvert.DeserializeObject<LadderEntry>(await response.Content.ReadAsStringAsync());

            result.Position.Should().Be(3);
        }

    }
}
