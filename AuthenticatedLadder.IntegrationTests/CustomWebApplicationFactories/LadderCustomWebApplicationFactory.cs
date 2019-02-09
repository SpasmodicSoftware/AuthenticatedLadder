using AuthenticatedLadder.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace AuthenticatedLadder.IntegrationTests.CustomWebApplicationFactories
{
    public class LadderCustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public string JWTHeaderName { get; private set; }
        public string JWTDecodeSecret { get; private set; }
        public string LadderRepositorySettingsLength { get; private set; }
        public string __CURRENT_DB_FILENAME__ { get; private set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            JWTHeaderName = "TestHeader";
            JWTDecodeSecret = "VerySecretTest";
            LadderRepositorySettingsLength = "5";
            __CURRENT_DB_FILENAME__ = $"{DateTime.UtcNow.ToString("yyyyMMddTHHmmss")}.db";

            builder.ConfigureAppConfiguration(configBuilder =>
            {
                var configs = new Dictionary<string, string>
                {
                    {"JWT:HeaderName", JWTHeaderName},
                    {"JWT:DecodeSecret", JWTDecodeSecret },
                    {"LadderRepositorySettings:Length", LadderRepositorySettingsLength }
                };
                configBuilder.AddInMemoryCollection(configs);
            });
            builder.ConfigureServices(services =>
            {
                services.AddDbContext<LadderDBContext>(options => options.UseSqlite($"DataSource={__CURRENT_DB_FILENAME__}"));

                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<LadderDBContext>();

                    db.Database.Migrate();
                    db.Database.EnsureCreated();

                    Utilities.PrepareDatabaseForTest(db);
                }
            });
        }
    }
}
