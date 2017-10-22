﻿using Autofac;
using BlogCore.BlogContext.Infrastructure;
using BlogCore.BlogContext.UseCases.GetBlogsByUserName;
using BlogCore.Infrastructure.EfCore;

namespace BlogCore.BlogContext
{
    public class BlogUseCaseModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(x =>
                DbContextHelper.BuildDbContext<BlogDbContext>(
                    x.ResolveKeyed<string>("MainDbConnectionString")))
                .SingleInstance();

            builder.RegisterType<GetBlogsByUserNameInteractor>()
                .AsSelf()
                .SingleInstance();
        }
    }
}