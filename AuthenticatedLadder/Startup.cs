using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthenticatedLadder.ExtensionMethods;
using AuthenticatedLadder.Middlewares;
using AuthenticatedLadder.Persistence;
using AuthenticatedLadder.Services.TokenDecoder;
using AuthenticatedLadder.Middlewares.ErrorHandling;

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

            services.AddTransient<ITokenDecoderService, JWTService>();

            services.AddOptions<JWTPayloadMiddlewareSettings>()
                .Configure(o =>
                {
                    o.HeaderName = Configuration["JWT:HeaderName"];
                    o.DecodeSecret = Configuration["JWT:DecodeSecret"];
                })
                .Validate(o => o.isValidConfiguration(), "JWTPayloadMiddlewareSettings not properly set");
            services.AddOptions<LadderRepositorySettings>()
                .Configure(o => { o.Length = int.Parse(Configuration["LadderRepositorySettings:Length"]); })
                .Validate(o => o.IsValidConfiguration(), "LadderRepositorySettings not properly set");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ConfigureExceptionHandler();
            app.UseJWTPayloadMiddleware();
            app.UseMvc();
        }
    }
}
