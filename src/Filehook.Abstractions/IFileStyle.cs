namespace Filehook.Abstractions
{
    public class FileStyle
    {
        public FileStyle(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
