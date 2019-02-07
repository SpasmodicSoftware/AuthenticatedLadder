using AuthenticatedLadder.IntegrationTests.CustomWebApplicationFactories;
using GenericAuthenticatedLadder;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Fact(Skip = "TODO")]
        public void Test()
        {
            _factory.CreateClient();
            throw new NotImplementedException();
        }
    }
}
