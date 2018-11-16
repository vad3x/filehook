using System.Collections.Generic;
using System.Threading.Tasks;

namespace Filehook.Abstractions
{
    public interface IBlobMetadataExtender
    {
        Task ExtendAsync(Dictionary<string, string> metadata, FilehookFileInfo fileInfo);
    }
}
