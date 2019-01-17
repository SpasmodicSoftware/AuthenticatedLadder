using GenericAuthenticatedLadder;
using Jose;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace AuthenticatedLadder.IntegrationTests.Controllers
{
    public class AuthenticatedEchoControllerTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private WebApplicationFactory<Startup> _factory;
        private const string _testHeaderName = "X-Test-Header";

        public AuthenticatedEchoControllerTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_ReturnsUnauthorizedIfNoAuthorizationHeaderPassed()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/echo");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Get_ReturnsUnauthorizedIfNoValidJWTPassedAsAuthorizationHeader()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(_testHeaderName, "23409238nuc2ur0ifope");

            var response = await client.GetAsync("/echo");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Get_ReturnsYourPayloadIfPayloadSignedWithValidJWTAuthentication()
        {
            var testSecretKey = "TestSecretKey";
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.AddInMemoryCollection(
                        new Dictionary<string, string>
                        {
                           {"JWT:DecodeSecret", testSecretKey },
                           {"JWT:HeaderName", _testHeaderName }
                        });
                });
            })
            .CreateClient();

            var payload = new JObject()
            {
                {"itworks", true }
            };

            var token = JWT.Encode(payload, testSecretKey,
                JweAlgorithm.PBES2_HS256_A128KW, JweEncryption.A256CBC_HS512);

            client.DefaultRequestHeaders.Add(_testHeaderName, token);

            var response = await client.GetAsync("/echo");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responsePayload = JsonConvert.DeserializeObject<JObject>(
                await response.Content.ReadAsStringAsync());

            Assert.True(JObject.DeepEquals(payload, responsePayload));

        }

        [Fact]
        public async Task Get_IsNoLongerUsingAuthorizationHeader()
        {
            var testSecretKey = "TestSecretKey";
            var client = _factory.WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((builderContext, config) =>
                    {
                        config.AddInMemoryCollection(
                            new Dictionary<string, string>
                            {
                                {"JWT:DecodeSecret", testSecretKey },
                                {"JWT:HeaderName", _testHeaderName }
                            });
                    });
                })
                .CreateClient();

            var payload = new JObject()
            {
                {"itworks", true }
            };

            var token = JWT.Encode(payload, testSecretKey,
                JweAlgorithm.PBES2_HS256_A128KW, JweEncryption.A256CBC_HS512);

            client.DefaultRequestHeaders.Add("Authorization", token);

            var response = await client.GetAsync("/echo");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

    }
}
