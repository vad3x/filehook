using Filehook.Abstractions;

namespace Filehook.Storages.Ssh
{
    public class SftpStorageOptions : StorageOptions
    {
        public string HostName { get; set; }

        public int Port { get; set; } = 22;

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Root { get; set; }
    }
}
