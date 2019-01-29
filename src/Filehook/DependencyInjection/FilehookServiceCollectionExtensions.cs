using System;

using Filehook;
using Filehook.Abstractions;
using Filehook.Core.DependencyInjection;
using Filehook.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FilehookServiceCollectionExtensions
    {
        public static IFilehookBuilder AddFilehook(this IServiceCollection services, Action<FilehookOptions> setupAction = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var builder = new FilehookBuilder(services);

            if (setupAction != null)
            {
                builder.Services.Configure(setupAction);
            }

            builder.Services.AddTransient<IFilehookService, RegularFilehookService>();

            return builder;
        }
    }
}
