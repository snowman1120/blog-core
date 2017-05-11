﻿using System.Threading.Tasks;
using MediatR;

namespace BlogCore.Core.Blogs.GetBlog
{
    public class GetBlogInteractor : IAsyncRequestHandler<GetBlogRequestMsg, GetBlogResponseMsg>
    {
        private readonly IRepository<Blog> _blogRepo;

        public GetBlogInteractor(IRepository<Blog> blogRepo)
        {
            _blogRepo = blogRepo;
        }

        public async Task<GetBlogResponseMsg> Handle(GetBlogRequestMsg message)
        {
            var blog = await _blogRepo.GetByIdAsync(message.Id);
            return new GetBlogResponseMsg
            {
                Id = blog.Id,
                Title = blog.Title,
                Description = blog.Description,
                Image = blog.Image,
                Theme = blog.Theme
            };
        }
    }
}