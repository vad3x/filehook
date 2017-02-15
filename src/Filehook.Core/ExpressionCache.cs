using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Filehook.Core
{
    internal static class ExpressionCache<TFunc>
    {
        private static readonly ConcurrentDictionary<Expression<TFunc>, TFunc> _cache = new ConcurrentDictionary<Expression<TFunc>, TFunc>();

        public static TFunc CachedCompile(Expression<TFunc> targetSelector)
        {
            return _cache.GetOrAdd(targetSelector, key => targetSelector.Compile());
        }
    }
}
