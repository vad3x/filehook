using System.Threading.Tasks;

namespace Filehook.Proccessors.Image.ImageSharpProccessor
{
    public class ImageSharpImageProccessorOptions
    {
        public ParallelOptions ParallelOptions { get; set; } = new ParallelOptions { MaxDegreeOfParallelism = 1 };
    }
}