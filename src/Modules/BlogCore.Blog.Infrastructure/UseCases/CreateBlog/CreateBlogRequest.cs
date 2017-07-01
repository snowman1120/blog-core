﻿using BlogCore.Core;
using MediatR;

namespace BlogCore.Blog.Infrastructure.UseCases.CreateBlog
{
    public class CreateBlogRequestMsg : IMesssage, IRequest<CreateBlogResponse>
    {
        public CreateBlogRequestMsg(
            string title, 
            string description,
            string theme, 
            int? postsPerPage, 
            int? daysToComment, 
            bool? moderateComments)
        {
            Title = title;
            Description = description;
            Theme = theme ?? "default";
            PostsPerPage = postsPerPage ?? 10;
            DaysToComment = daysToComment ?? 5;
            ModerateComments = moderateComments ?? true;
        }

        public string Title { get; }
        public string Description { get; }
        public string Theme { get; }
        public int PostsPerPage { get; }
        public int DaysToComment { get; }
        public bool ModerateComments { get; }
    }
}