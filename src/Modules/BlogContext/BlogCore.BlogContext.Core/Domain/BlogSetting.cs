﻿using System;
using System.Collections.Generic;
using BlogCore.Core;
using System.ComponentModel.DataAnnotations;

namespace BlogCore.BlogContext.Core.Domain
{
    public class BlogSetting : ValueObjectBase
    {
        private BlogSetting()
        {
        }

        public BlogSetting(Guid blogSettingId, int postsPerPage, int daysToComment, bool moderateComments)
        {
            BlogSettingId = blogSettingId;
            PostsPerPage = postsPerPage;
            DaysToComment = daysToComment;
            ModerateComments = moderateComments;
        }

        [Key]
        public Guid BlogSettingId { get; private set; }
        public int PostsPerPage { get; private set; }
        public int DaysToComment { get; private set; }
        public bool ModerateComments { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return PostsPerPage;
            yield return DaysToComment;
            yield return ModerateComments;
        }
    }
}