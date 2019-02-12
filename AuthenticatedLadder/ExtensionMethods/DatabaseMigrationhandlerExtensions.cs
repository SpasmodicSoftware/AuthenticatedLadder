using AuthenticatedLadder.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using System;

namespace AuthenticatedLadder.ExtensionMethods
{
    public static class DatabaseMigrationhandlerExtensions
    {
        public static void MigrateDatabase(this IApplicationBuilder app)
        {
            try
            {
                var dbContext = (LadderDBContext)app.ApplicationServices.GetService(typeof(LadderDBContext));
                if (dbContext != null)
                {
                    dbContext.Database.Migrate();
                }
            }
            catch (InvalidOperationException)
            {

            }
        }
    }
}
