namespace Filehook.Storages.Ssh
{
    public class SftpStorageOptions
    {
        public string Name { get; set; } = SshConsts.SftpStorageName;

        public string HostName { get; set; }

        public int Port { get; set; } = 22;

        public string UserName { get; set; }

        public string Password { get; set; }

        public string BasePath { get; set; }

        public string RequestRootUrl { get; set; } = string.Empty;
    }
}
