using Microsoft.Extensions.DependencyInjection;

namespace Filehook.Core.DependencyInjection
{
    public interface IFilehookBuilder
    {
        IServiceCollection Services { get; }
    }
}
