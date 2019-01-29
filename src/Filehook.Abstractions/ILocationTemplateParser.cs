using System;

namespace Filehook.Abstractions
{
    [Obsolete("Use options")]
    public interface ILocationTemplateParser
    {
        string SetRoot(string locationTemplate, string rootLocation);
    }
}
