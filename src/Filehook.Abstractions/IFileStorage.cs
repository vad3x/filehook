using System.IO;
using System.Threading.Tasks;

namespace Filehook.Abstractions
{
    public interface IFileStorage
    {
        string Name { get; }

        string GetUrl(string relativeLocation);

        Task<bool> ExistsAsync(string relativeLocation);

        Task<string> SaveAsync(string relativeLocation, Stream stream);
    }
}
