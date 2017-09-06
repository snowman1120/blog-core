﻿using BlogCore.BlogContext.Domain;
using MediatR;

namespace BlogCore.BlogContext.UseCases.Crud
{
    public class BlogSettingChangedHandler : INotificationHandler<BlogCreated>
    {
        public void Handle(BlogCreated notification)
        {
            // TODO: send an email to notify for administrator
            // TODO: ...
        }
    }
}