using Filehook.Abstractions;

namespace Filehook.Storages.S3
{
    public class S3StorageOptions : StorageOptions
    {
        public override string Name { get; set; } = S3Consts.S3StorageName;

        public string Protocol { get; set; } = "https";

        public string Region { get; set; } = "us-east-1";

        public string HostName { get; set; } = "amazonaws.com";

        public string ProxyUri { get; set; }

        public string BucketName { get; set; }

        public string AccessKeyId { get; set; }

        public string SecretAccessKey { get; set; }
    }
}
