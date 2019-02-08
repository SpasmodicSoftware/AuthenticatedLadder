using AuthenticatedLadder.Logging;
using AuthenticatedLadder.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AuthenticatedLadder.IntegrationTests.CustomWebApplicationFactories
{
    public class LadderCustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

            builder.ConfigureServices(services =>
            {

                services.AddDbContext<LadderDBContext>(options => options.UseSqlite("DataSource=:memory:"));

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<LadderDBContext>();

                    // Ensure the database is created.
                    db.Database.EnsureCreated();
                    Utilities.PrepareDatabaseForTest(db);

                }
            });
        }
    }
}
