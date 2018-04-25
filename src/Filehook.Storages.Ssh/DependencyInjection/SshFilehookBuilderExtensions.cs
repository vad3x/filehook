using System;
using Filehook.Abstractions;
using Filehook.Core.DependencyInjection;
using Filehook.Storages.Ssh;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SshFilehookBuilderExtensions
    {
        public static IFilehookBuilder AddSftpStorage(this IFilehookBuilder builder, Action<SftpStorageOptions> setupAction = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (setupAction != null)
            {
                builder.Services.Configure(setupAction);
            }

            builder.Services.AddTransient<IFileStorage, SftpStorage>();

            return builder;
        }
    }
}
