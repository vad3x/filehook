using Filehook.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Filehook.Core
{
    public class AttributeFileStyleResolver : IFileStyleResolver
    {
        public IEnumerable<FileStyle> Resolve<TEntity>(Expression<Func<TEntity, string>> propertyExpression)
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

            var styles = memberExpression.Member.GetCustomAttributes<HasFileStyleAttribute>()
                .Select(x => x.Style)
                .ToList();

            if (styles.GroupBy(s => s.Name).Any(g => g.Count() > 1))
            {
                throw new ArgumentException($"'{propertyExpression}': has dublicate style attributes");
            }

            if (!styles.Any(s => s.Name == FilehookConsts.OriginalStyleName))
            {
                styles.Add(new FileStyle(FilehookConsts.OriginalStyleName));
            }

            return styles;
        }
    }
}
