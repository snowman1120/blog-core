﻿using BlogCore.Shared.v1.Blog;
using BlogCore.Shared.v1.Guard;
using BlogCore.Shared.v1.Usecase;
using BlogCore.Shared.v1.ValidationModel;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogCore.Modules.BlogContext.Usecases
{
    public class GetBlogByUserNameUseCase : IUseCase<GetMyBlogsRequest, PaginatedBlogResponse>
    {
        private readonly IValidator<GetMyBlogsRequest> _validator;

        public GetBlogByUserNameUseCase(IValidator<GetMyBlogsRequest> validator)
        {
            _validator = validator.NotNull();
        }

        public async Task<PaginatedBlogResponse> ExecuteAsync(GetMyBlogsRequest request)
        {
            await _validator.HandleValidation(request);

            // TODO: get from database
            // ...

            var pager = new PaginatedBlogResponse
            {
                TotalItems = 1,
                TotalPages = 1,
            };
            pager.Items.AddRange(new List<BlogDto> {
                        new BlogDto
                        {
                            Id = Guid.NewGuid().ToString(),
                            Title = "My blog",
                            Description = "This is my blog",
                            Image = "/images/my-blog.png",
                            Theme = 1
                        }
                    });

            return await Task.FromResult(pager);
        }
    }
}
