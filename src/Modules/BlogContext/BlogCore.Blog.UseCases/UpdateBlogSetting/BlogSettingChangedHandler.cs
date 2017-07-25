﻿using BlogCore.Blog.Domain;
using BlogCore.Blog.Infrastructure;
using BlogCore.Core;
using MediatR;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BlogCore.Blog.UseCases.UpdateBlogSetting
{
    public class BlogSettingChangedHandler : IAsyncNotificationHandler<BlogSettingChanged>
    {
        private readonly BlogDbContext _dbContext;

        public BlogSettingChangedHandler(BlogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(BlogSettingChanged notification)
        {
            var blodSetting = await _dbContext.Set<BlogSetting>().FirstOrDefaultAsync(x => x.Id == notification.OldBlogSettingId);
            if (blodSetting == null)
            {
                throw new CoreException($"Could not found the BlogSetting #{notification.OldBlogSettingId}");
            }
            _dbContext.Entry(blodSetting).State = EntityState.Deleted;
            await _dbContext.SaveChangesAsync();
        }
    }
}