using System;

namespace Filehook.Abstractions
{
    public class FilehookOptions
    {
        public string DefaultStorageName { get; set; }

        public Func<string> NewKey { get; set; } = () => StringGenerator.Generate(24);
    }
}
