﻿using System;
using System.Threading.Tasks;
using BlogCore.AccessControl.Domain;
using BlogCore.Core;
using Microsoft.EntityFrameworkCore;

namespace BlogCore.AccessControl.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        private readonly IdentityServerDbContext _dbContext;

        public UserRepository(IdentityServerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AppUser> GetByIdAsync(Guid id)
        {
            var userSet = _dbContext.Set<AppUser>();
            var user = await userSet.SingleOrDefaultAsync(x => Guid.Parse(x.Id) == id);
            return user;
        }

        public async Task UpdateUserProfile(Guid id, string givenName, string familyName, string bio, string company,
            string location)
        {
            var user = await _dbContext.Set<AppUser>().SingleOrDefaultAsync(x => Guid.Parse(x.Id) == id);
            if (user == null)
                throw new CoreException($"Could not find out UserProfile with id={id}.");

            user.GivenName = givenName;
            user.FamilyName = familyName;
            user.Bio = bio;
            user.Company = company;
            user.Location = location;
            await _dbContext.SaveChangesAsync();
        }
    }
}