﻿using Autofac;
using BlogCore.AccessControlContext.Infrastructure;
using BlogCore.Core;
using BlogCore.Infrastructure.EfCore;

namespace BlogCore.AccessControl
{
    public class AccessControlUseCaseModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            // security context
            builder.RegisterType<SecurityContextProvider>()
                .As<ISecurityContext>()
                .As<ISecurityContextPrincipal>()
                .InstancePerLifetimeScope();

            builder.Register(x =>
                DbContextHelper.BuildDbContext<IdentityServerDbContext>(
                    x.ResolveKeyed<string>("MainDbConnectionString")))
                .SingleInstance();

            builder.RegisterType<UserRepository>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}