using System;

using Filehook.Abstractions.Stores;
using Filehook.Core.DependencyInjection;
using Filehook.Extensions.EntityFrameworkCore;
using Filehook.Extensions.EntityFrameworkCore.Stores;

using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FilehookServiceCollectionExtensions
    {
        public static IFilehookBuilder AddEntityFrameworkStores(this IFilehookBuilder builder, Action<DbContextOptionsBuilder> dbContextOptionsAction = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddDbContext<FilehookDbContext>(dbContextOptionsAction);

            builder.Services.AddTransient<IFilehookStore, EntityFrameworkStore>();

            return builder;
        }
    }
}
