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

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<LadderDBContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILoggerAdapter<TStartup>>();

                    // Ensure the database is created.
                    db.Database.EnsureCreated();

                    try
                    {
                        Utilities.PrepareDatabaseForTest(db);
                    }
                    catch (Exception)
                    {
                        //logger.LogError(ex, $"An error occurred seeding the " +
                        //"database with test messages. Error: {ex.Message}");
                    }
                }
            });
        }
    }
}
