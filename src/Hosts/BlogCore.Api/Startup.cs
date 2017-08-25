﻿#region libs

using BlogCore.AccessControl.Domain;
using BlogCore.AccessControl.Domain.SecurityContext;
using BlogCore.AccessControl.Infrastructure.SecurityContext;
using BlogCore.AccessControl.UseCases;
using BlogCore.BlogContext;
using BlogCore.BlogContext.Infrastructure;
using BlogCore.Core;
using BlogCore.Infrastructure.AspNetCore;
using BlogCore.Infrastructure.EfCore;
using BlogCore.Post.UseCases;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace BlogCore.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Configuration = env.BuildConfiguration();
            Environment = env;

            // https://github.com/dotnet/corefx/issues/9158
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddCorsForBlog()
                .AddAuthorizationForBlog()
                .AddMvcForBlog(RegisteredAssemblies());

            services.AddOptions()
                .Configure<PagingOption>(Configuration.GetSection("Paging"));

            if (Environment.IsDevelopment())
                services.AddSwaggerForBlog();

            services.AddMediatR(RegisteredAssemblies());
            return services.InitServices(RegisteredAssemblies(), Configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"))
                .AddDebug();

            app.UseIdentityServerForBlog(OnTokenValidated)
                .UseStaticFiles(new StaticFileOptions
                {
                    OnPrepareResponse = ctx =>
                    {
                        var maxAge = new TimeSpan(7, 0, 0, 0);
                        ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                            "public,max-age=" + maxAge.TotalSeconds.ToString("0");
                    }
                })
                .UseCors("CorsPolicy")
                .UseMvc();

            if (env.IsDevelopment())
                app.UseSwaggerUiForBlog();
        }

        private static Assembly[] RegisteredAssemblies()
        {
            return new[]
            {
                typeof(BlogUseCaseModule).GetTypeInfo().Assembly,
                typeof(PostUseCaseModule).GetTypeInfo().Assembly,
                typeof(AccessControlUseCaseModule).GetTypeInfo().Assembly,
                typeof(Startup).GetTypeInfo().Assembly
            };
        }

        private static async Task OnTokenValidated(TokenValidatedContext context)
        {
            // get current principal
            var principal = context.Ticket.Principal;

            // get current claim identity
            var claimsIdentity = context.Ticket.Principal.Identity as ClaimsIdentity;

            // build up the id_token and put it into current claim identity
            var headerToken =
                context.Request.Headers["Authorization"][0].Substring(context.Ticket.AuthenticationScheme.Length + 1);
            claimsIdentity?.AddClaim(new Claim("id_token", headerToken));

            var securityContextInstance = context.HttpContext.RequestServices.GetService<ISecurityContext>();
            var securityContextPrincipal = securityContextInstance as ISecurityContextPrincipal;
            if (securityContextPrincipal == null)
                throw new ViolateSecurityException("Could not initiate the MasterSecurityContextPrincipal object.");
            securityContextPrincipal.Principal = principal;

            var blogRepoInstance = context.HttpContext.RequestServices.GetService<IEfRepository<BlogDbContext, BlogContext.Domain.Blog>>();
            var email = securityContextInstance.GetCurrentEmail();
            var blogs = await blogRepoInstance.ListAsync();
            var blog = blogs.FirstOrDefault(x => x.OwnerEmail == email);
            securityContextPrincipal.SetBlog(blog);

            await Task.FromResult(0);
        }
    }
}