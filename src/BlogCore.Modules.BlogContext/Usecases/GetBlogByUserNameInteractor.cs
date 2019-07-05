﻿using BlogCore.Shared.v1.Blog;
using BlogCore.Shared.v1.Usecase;
using BlogCore.Shared.v1.ValidationModel;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogCore.Modules.BlogContext.Usecases
{
    public class GetBlogByUsernameInteractor : IUseCase<GetMyBlogsRequest, PaginatedBlogDto>
    {
        private readonly IValidator<GetMyBlogsRequest> _validator;

        public GetBlogByUsernameInteractor(IValidator<GetMyBlogsRequest> validator)
        {
            _validator = validator;
        }

        public async Task<PaginatedBlogDto> HandleAsync(GetMyBlogsRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new Shared.v1.ValidationModel.ValidationException(validationResult.ToValidationResultModel());
            }

            // TODO: get from database
            // ...

            var pager = new PaginatedBlogDto
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
