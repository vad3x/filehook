using System;

namespace Filehook.Abstractions
{
    public class FilehookOptions
    {
        public Func<string> NewKey { get; set; } = () => StringGenerator.Generate(24);

        public FilehookAttachmentOptions AttachmentOptions { get; set; } = new FilehookAttachmentOptions();
    }
}
