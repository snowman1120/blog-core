﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BlogCore.Core;
using BlogCore.Infrastructure.Data;
using BlogCore.Web.Blogs;
using FluentValidation.AspNetCore;
using IdentityServer4.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace BlogCore.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            Environment = env;
        }

        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var builder = new ContainerBuilder();

            services.AddMvc()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssembly(typeof(BlogCoreDbContext).GetTypeInfo().Assembly));

            services.AddAuthorization()
                .AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy",
                        policy => policy.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials());
                });

            if (Environment.IsDevelopment())
                services.AddSwaggerGen(options =>
                {
                    options.DescribeAllEnumsAsStrings();
                    options.SwaggerDoc("v1", new Info
                    {
                        Title = "Blog Core",
                        Version = "v1",
                        Description = "Blog Core APIs"
                    });
                    options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                    {
                        Type = "oauth2",
                        Flow = "implicit",
                        TokenUrl = "http://localhost:8481/connect/token",
                        AuthorizationUrl = "http://localhost:8481/connect/authorize",
                        Scopes = new Dictionary<string, string> { { "blog_core_api", "My Blog Core API" } }
                    });
                });

            services.AddDbContext<BlogCoreDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MainDb")));

            services.AddMediatR(
                typeof(EntityBase).GetTypeInfo().Assembly,
                typeof(BlogCoreDbContext).GetTypeInfo().Assembly,
                typeof(Startup).GetTypeInfo().Assembly);

            // Core & Infra register
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>));

            // Web registers
            builder.RegisterType<BlogPresenter>().AsSelf();

            builder.Populate(services);
            return builder.Build().Resolve<IServiceProvider>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            JwtSecurityTokenHandler.DefaultInboundClaimFilter.Clear();
            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                AuthenticationScheme = "Bearer",
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                Authority = "http://localhost:8481",
                SaveToken = true,
                AllowedScopes = new[] { "blog_core_api" },
                RequireHttpsMetadata = false,
                JwtBearerEvents = new JwtBearerEvents
                {
                    OnTokenValidated = OnTokenValidated
                }
            });

            app.UseStaticFiles().UseCors("CorsPolicy");
            app.UseMvc();

            if (env.IsDevelopment())
                app.UseSwagger().UseSwaggerUI(
                    c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blog Core APIs");
                        c.ConfigureOAuth2("swagger", "secret".Sha256(), "swagger", "swagger");
                    });
        }

        private static async Task OnTokenValidated(TokenValidatedContext context)
        {
            // get current principal
            var principal = context.Ticket.Principal;

            // get current claim identity
            var claimsIdentity = context.Ticket.Principal.Identity as ClaimsIdentity;

            // build up the id_token and put it into current claim identity
            var headerToken = context.Request.Headers["Authorization"][0].Substring(context.Ticket.AuthenticationScheme.Length + 1);
            claimsIdentity?.AddClaim(new Claim("id_token", headerToken));

            // TODO: do something for building up the SecurityContext here
            // TODO: ...

            await Task.FromResult(0);
        }
    }
}