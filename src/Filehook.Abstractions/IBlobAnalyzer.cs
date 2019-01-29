using System.Collections.Generic;
using System.Threading.Tasks;

namespace Filehook.Abstractions
{
    public interface IBlobAnalyzer
    {
        Task AnalyzeAsync(Dictionary<string, string> metadata, FilehookFileInfo fileInfo);
    }
}
