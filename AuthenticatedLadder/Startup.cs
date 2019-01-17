﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthenticatedLadder.ExtensionMethods;
using AuthenticatedLadder.Middlewares;
using AuthenticatedLadder.Services.TokenDecoder;

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

            var verySecretKey = Configuration["JWT:Secret"];
            services.AddTransient<ITokenDecoderService>(s => new JWTService(verySecretKey));

            services.Configure<JWTPayloadMiddlewareSettings>(options => new JWTPayloadMiddlewareSettings
            {
                HeaderName = Configuration["JWT:HeaderName"]
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseJWTPayloadMiddleware();
            app.UseMvc();
        }
    }
}
