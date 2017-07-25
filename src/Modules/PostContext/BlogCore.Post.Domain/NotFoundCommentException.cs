﻿using System;
using BlogCore.Core;

namespace BlogCore.Post.Domain
{
    public class NotFoundCommentException : CoreException
    {
        public NotFoundCommentException(string message)
            : this(message, null)
        {
        }

        public NotFoundCommentException(string message, Exception innerEx)
            : base(message, innerEx)
        {
        }
    }
}