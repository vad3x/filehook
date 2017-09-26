namespace Filehook.Proccessors.Image.Abstractions
{
    public class ImageEncodeOptions
    {
        /// <summary>
        /// Must be between 0 and 100 (compression from max to min).
        /// </summary>
        public int Quality { get; set; } = 100;

        public string MimeType { get; set; }
    }
}
