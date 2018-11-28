using Filehook.Abstractions;

namespace Filehook.Storages.FileSystem
{
    public class FileSystemStorageOptions : StorageOptions
    {
        public virtual string Root { get; set; } = string.Empty;
    }
}
