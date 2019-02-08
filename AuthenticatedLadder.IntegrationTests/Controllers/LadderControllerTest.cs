using AuthenticatedLadder.IntegrationTests.CustomWebApplicationFactories;
using GenericAuthenticatedLadder;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AuthenticatedLadder.Middlewares.JWTPayload;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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

        [Fact]
        public async Task GetTopForLadder_ReturnsUnauthorizedIfNotValidJWTPayload()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/ladder/myLadder");
            
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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
