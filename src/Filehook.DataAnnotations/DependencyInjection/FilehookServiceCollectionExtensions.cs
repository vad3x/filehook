using System;
using Filehook.Abstractions;
using Filehook.Core.DependencyInjection;
using Filehook.DataAnnotations;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FilehookServiceCollectionExtensions
    {
        public static IFilehookBuilder AddDataAnnotations(this IFilehookBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddTransient<IFileStorageNameResolver, AttributeFileStorageNameResolver>();
            builder.Services.AddTransient<IEntityIdResolver, AttributeEntityIdResolver>();

            return builder;
        }
    }
}
