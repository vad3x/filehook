using Filehook.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Filehook.Core.Internal
{
    internal class FilehookBuilder : IFilehookBuilder
    {
        public FilehookBuilder(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
