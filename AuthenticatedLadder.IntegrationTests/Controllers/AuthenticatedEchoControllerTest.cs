using GenericAuthenticatedLadder;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace AuthenticatedLadder.IntegrationTests.Controllers
{
    public class AuthenticatedEchoControllerTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private WebApplicationFactory<Startup> _factory;

        public AuthenticatedEchoControllerTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_ReturnsUnauthorizedIfNoValidJWT()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/echo");

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact(Skip = "TODO")]
        public async Task Get_EchoReturnsYourPayloadIfAuthenticated()
        {
            throw new NotImplementedException();
        }
    }
}
