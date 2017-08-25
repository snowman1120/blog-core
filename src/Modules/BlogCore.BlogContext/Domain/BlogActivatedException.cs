﻿using System;
using BlogCore.Core;

namespace BlogCore.BlogContext.Domain
{
    public class BlogActivatedException : CoreException
    {
        public BlogActivatedException(string message)
            : this(message, null)
        {
        }

        public BlogActivatedException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }
    }
}