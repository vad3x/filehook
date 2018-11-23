using Filehook.Abstractions;

namespace Filehook.Storages.FileSystem
{
    public class FileSystemStorageOptions : StorageOptions
    {
        public override string Name { get; set; } = FileSystemConsts.FileSystemStorageName;

        public virtual string Root { get; set; } = string.Empty;
    }
}
