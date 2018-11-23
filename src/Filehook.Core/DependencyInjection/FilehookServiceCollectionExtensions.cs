using System;

using Filehook.Abstractions;
using Filehook.Core;
using Filehook.Core.DependencyInjection;
using Filehook.Core.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FilehookServiceCollectionExtensions
    {
        public static IFilehookBuilder AddFilehookCore(this IServiceCollection services, Action<FilehookOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var builder = new FilehookBuilder(services);

            builder.Services.Configure(setupAction);

            builder.Services.AddTransient<IFilehookService, RegularFilehookService>();

            return builder;
        }
    }
}
