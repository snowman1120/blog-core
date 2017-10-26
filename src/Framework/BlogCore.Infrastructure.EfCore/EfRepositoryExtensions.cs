﻿using BlogCore.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace BlogCore.Infrastructure.EfCore
{
    public static class EfRepositoryExtensions
    {
        public static IObservable<PaginatedItem<TEntity>> ListStream<TDbContext, TEntity>(
            this IEfRepository<TDbContext, TEntity> repo,
            Expression<Func<TEntity, bool>> filter = null,
            Criterion criterion = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
            where TDbContext : DbContext
            where TEntity : EntityBase
        {
            if (criterion.PageSize < 1 || criterion.PageSize > criterion.DefaultPagingOption.PageSize)
            {
                criterion.SetPageSize(criterion.DefaultPagingOption.PageSize);
            }

            var queryable = repo.DbContext.Set<TEntity>().AsNoTracking() as IQueryable<TEntity>;
            var totalRecord = queryable.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecord / criterion.PageSize);

            foreach (var includeProperty in includeProperties)
            {
                queryable = queryable.Include(includeProperty);
            }

            IQueryable<TEntity> criterionQueryable = null;
            if (criterion != null && filter != null)
            {
                criterionQueryable = queryable.Skip(criterion.CurrentPage * criterion.PageSize)
                    .Take(criterion.PageSize)
                    .Where(filter);
            }
            else if (criterion == null)
            {
                criterionQueryable = queryable.Skip(criterion.CurrentPage * criterion.PageSize)
                    .Take(criterion.PageSize);
            }
            else if (filter == null)
            {
                criterionQueryable = queryable.Where(filter);
            }

            return Observable.FromAsync(async () =>
                new PaginatedItem<TEntity>(
                    totalRecord,
                    totalPages,
                    await criterionQueryable.ToListAsync()));
        }

        public static async Task<PaginatedItem<TResponse>> QueryAsync<TDbContext, TEntity, TResponse>(
            this IEfRepository<TDbContext, TEntity> repo,
            Criterion criterion,
            Expression<Func<TEntity, TResponse>> selector,
            params Expression<Func<TEntity, object>>[] includeProperties)
            where TDbContext : DbContext
            where TEntity : EntityBase
        {
            return await GetDataAsync(repo, criterion, selector, null, includeProperties);
        }

        public static async Task<PaginatedItem<TResponse>> FindAllAsync<TDbContext, TEntity, TResponse>(
            this IEfRepository<TDbContext, TEntity> repo,
            Criterion criterion,
            Expression<Func<TEntity, TResponse>> selector,
            Expression<Func<TEntity, bool>> filter,
            params Expression<Func<TEntity, object>>[] includeProperties)
            where TDbContext : DbContext
            where TEntity : EntityBase
        {
            return await GetDataAsync(repo, criterion, selector, filter, includeProperties);
        }

        public static async Task<TEntity> FindOneAsync<TDbContext, TEntity>(
            this IEfRepository<TDbContext, TEntity> repo,
            Expression<Func<TEntity, bool>> filter,
            params Expression<Func<TEntity, object>>[] includeProperties)
            where TDbContext : DbContext
            where TEntity : EntityBase
        {
            var dbSet = repo.DbContext.Set<TEntity>() as IQueryable<TEntity>;
            foreach (var includeProperty in includeProperties)
            {
                dbSet = dbSet.Include(includeProperty);
            }

            return await dbSet.FirstOrDefaultAsync(filter);
        }

        private static async Task<PaginatedItem<TResponse>> GetDataAsync<TDbContext, TEntity, TResponse>(
            IEfRepository<TDbContext, TEntity> repo,
            Criterion criterion,
            Expression<Func<TEntity, TResponse>> selector,
            Expression<Func<TEntity, bool>> filter = null,
            params Expression<Func<TEntity, object>>[] includeProperties)
            where TDbContext : DbContext
            where TEntity : EntityBase
        {
            if (criterion.PageSize < 1 || criterion.PageSize > criterion.DefaultPagingOption.PageSize)
            {
                criterion.SetPageSize(criterion.DefaultPagingOption.PageSize);
            }

            var queryable = repo.DbContext.Set<TEntity>() as IQueryable<TEntity>;
            var totalRecord = await queryable.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecord / criterion.PageSize);

            foreach (var includeProperty in includeProperties)
            {
                queryable = queryable.Include(includeProperty);
            }

            if (filter != null)
                queryable = queryable.Where(filter);

            if (!string.IsNullOrWhiteSpace(criterion.SortBy))
            {
                var isDesc = string.Equals(criterion.SortOrder, "desc", StringComparison.OrdinalIgnoreCase) ? true : false;
                queryable = queryable.OrderByPropertyName(criterion.SortBy, isDesc);
            }

            if (criterion.CurrentPage > totalPages)
            {
                criterion.SetCurrentPage(totalPages);
            }

            var results = await queryable
                .Skip(criterion.CurrentPage * criterion.PageSize)
                .Take(criterion.PageSize)
                .AsNoTracking()
                .Select(selector)
                .ToListAsync();

            return new PaginatedItem<TResponse>(totalRecord, totalPages, results);
        }
    }
}