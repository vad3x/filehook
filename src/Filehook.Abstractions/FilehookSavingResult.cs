namespace Filehook.Abstractions
{
    public class FilehookSavingResult
    {
        public string Location { get; set; }

        public string Url { get; set; }

        public object ProccessingMeta { get; set; }
    }
}