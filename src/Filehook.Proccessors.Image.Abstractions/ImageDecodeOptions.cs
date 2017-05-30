namespace Filehook.Proccessors.Image.Abstractions
{
    public class ImageDecodeOptions
    {
        /// <summary>
        /// Must be between 0 and 100 (compression from max to min).
        /// </summary>
        public int Quality { get; set; } = 100;
    }
}