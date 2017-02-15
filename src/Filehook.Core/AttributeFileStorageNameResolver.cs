﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Filehook.Abstractions;
using Microsoft.Extensions.Options;

namespace Filehook.Core
{
    public class AttributeFileStorageNameResolver : IFileStorageNameResolver
    {
        private readonly FileStorageNameResolverOptions _options;

        public AttributeFileStorageNameResolver(IOptions<FileStorageNameResolverOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options.Value;
        }

        public string Resolve<TEntity>(Expression<Func<TEntity, string>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException($"'{propertyExpression}': is not a valid expression for this method");
            }

            var storageName = memberExpression.Member.GetCustomAttributes<UseStorageAttribute>()
                .Select(x => x.Name)
                .FirstOrDefault();

            if (storageName == null)
            {
                storageName = _options.DefaultStorageName;
            }

            return storageName;
        }
    }
}
