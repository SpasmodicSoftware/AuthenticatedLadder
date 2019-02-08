using AuthenticatedLadder.Logging;
using AuthenticatedLadder.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using AuthenticatedLadder.Middlewares.JWTPayload;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AuthenticatedLadder.IntegrationTests.CustomWebApplicationFactories
{
    public class LadderCustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(configBuilder =>
            {
                var configs = new Dictionary<string, string>
                {
                    {"JWT:HeaderName", "TestHeader" },
                    //{ },
                    //{ }
                };
                configBuilder.AddInMemoryCollection(configs);
            });
            builder.ConfigureServices(services =>
            {
                services.AddDbContext<LadderDBContext>(options => options.UseSqlite("DataSource=:memory:"));

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<LadderDBContext>();

                    db.Database.EnsureCreated();
                    Utilities.PrepareDatabaseForTest(db);
                }
            });
        }
    }
}
