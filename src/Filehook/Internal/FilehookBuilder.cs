using Filehook.Core.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Filehook.Internal
{
    internal class FilehookBuilder : IFilehookBuilder
    {
        public FilehookBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IServiceCollection Services { get; }
    }
}
