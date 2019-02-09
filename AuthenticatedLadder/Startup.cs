using AuthenticatedLadder.DomainModels;
using AuthenticatedLadder.ExtensionMethods;
using AuthenticatedLadder.Logging;
using AuthenticatedLadder.Middlewares.JWTPayload;
using AuthenticatedLadder.Persistence;
using AuthenticatedLadder.Services.JWTPayloadHolder;
using AuthenticatedLadder.Services.Ladder;
using AuthenticatedLadder.Services.TokenDecoder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GenericAuthenticatedLadder
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddScoped<IJWTPayloadHolder, JWTPayloadHolder>();

            services.AddTransient(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));
            services.AddTransient<ITokenDecoderService, JWTService>();
            services.AddTransient<ILadderService, LadderService>();
            services.AddTransient<ILadderRepository, LadderRepository>();


            services.AddOptions<JWTPayloadMiddlewareSettings>()
                .Configure(o =>
                {
                    o.HeaderName = Configuration["JWT:HeaderName"];
                    //TODO questo deve diventare env.DECODE_SECRET per docker
                    o.DecodeSecret = Configuration["JWT:DecodeSecret"];
                })
                .Validate(o => o.IsValidConfiguration(), "JWTPayloadMiddlewareSettings not properly set");
            services.AddOptions<LadderRepositorySettings>()
                .Configure(o => { o.Length = int.Parse(Configuration["LadderRepositorySettings:Length"]); })
                .Validate(o => o.IsValidConfiguration(), "LadderRepositorySettings not properly set");

            ////TODO Legegre da config
            //var connStr = "DataSource=file.db";
            //services.AddDbContext<LadderDBContext>(opt => opt.UseSqlite(connStr));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseErrorHandlingMiddleware();
            app.UseJWTPayloadMiddleware();
            app.UseMvc();
        }
    }
}
