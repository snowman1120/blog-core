﻿using BlogCore.Blog.Domain;
using MediatR;

namespace BlogCore.Blog.UseCases.CreateBlog
{
    public class BlogCreatedHandler : INotificationHandler<BlogCreated>
    {
        public void Handle(BlogCreated notification)
        {
            // TODO: send an email to notify for administrator
            // TODO: ...
        }
    }
}