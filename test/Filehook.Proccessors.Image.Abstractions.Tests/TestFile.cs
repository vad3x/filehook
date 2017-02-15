using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Filehook.Proccessors.Image.Abstractions.Tests
{
    public class TestFile
    {
        private static readonly ConcurrentDictionary<string, TestFile> Cache = new ConcurrentDictionary<string, TestFile>();

        private static readonly string ImagesDirectory = GetImagesDirectory();

        private TestFile(string file)
        {
            using (var sourceStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                using (var memoryStream = new MemoryStream())
                {
                    sourceStream.CopyTo(memoryStream);
                    Bytes = memoryStream.ToArray();
                }
            } 
        }

        public static TestFile Create(string file)
        {
            //return Cache.GetOrAdd(file, (string fileName) => new TestFile(GetPath(file)));
            return new TestFile(GetPath(file));
        }

        public static string GetPath(string file)
        {
            return Path.Combine(ImagesDirectory, file);
        }

        public byte[] Bytes { get; private set; }

        private static string GetImagesDirectory()
        {
            var directories = new List<string> {
                 "TestImages/", // Here for code coverage tests.
                  "../Filehook.Proccessors.Image.Abstractions.Tests/TestImages/", // from travis/build script
                  "test/Filehook.Proccessors.Image.Abstractions.Tests/TestImages/", // for vs code
                  "../../../Filehook.Proccessors.Image.Abstractions.Tests/TestImages/", // from Sandbox46
                  "../../../../TestImages/"
            };

            directories = directories.SelectMany(x => new[]
                                     {
                                         Path.GetFullPath(x)
                                     }).ToList();

            AddFormatsDirectoryFromTestAssebmlyPath(directories);

            var directory = directories.FirstOrDefault(x => Directory.Exists(x));

            if (directory != null)
            {
                return directory;
            }

            throw new System.Exception($"Unable to find Formats directory at any of these locations [{string.Join(", ", directories)}]");
        }

        private static void AddFormatsDirectoryFromTestAssebmlyPath(List<string> directories)
        {
            string assemblyLocation = typeof(TestFile).GetTypeInfo().Assembly.Location;
            assemblyLocation = Path.GetDirectoryName(assemblyLocation);

            if (assemblyLocation != null)
            {
                string dirFromAssemblyLocation = Path.Combine(assemblyLocation, "../../../TestImages/");
                dirFromAssemblyLocation = Path.GetFullPath(dirFromAssemblyLocation);
                directories.Add(dirFromAssemblyLocation);
            }
        }
    }
}
