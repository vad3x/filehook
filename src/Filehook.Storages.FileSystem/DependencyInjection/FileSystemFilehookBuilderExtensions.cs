using Filehook.Abstractions;
using Filehook.Core.DependencyInjection;
using Filehook.Storages.FileSystem;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FileSystemFilehookBuilderExtensions
    {
        public static IFilehookBuilder AddFileSystemStorage(this IFilehookBuilder builder, Action<FileSystemStorageOptions> setupAction = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (setupAction != null)
            {
                builder.Services.Configure(setupAction);
            }

            builder.Services.AddTransient<IFileStorage, FileSystemStorage>();

            return builder;
        }
    }
}
