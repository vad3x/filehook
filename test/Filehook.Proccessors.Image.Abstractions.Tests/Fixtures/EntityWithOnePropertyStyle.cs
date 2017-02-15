namespace Filehook.Proccessors.Image.Abstractions.Tests.Fixtures
{
    public class EntityWithOnePropertyStyle
    {
        [HasImageStyle("regular", 100, 100)]
        public string ImageName { get; set; }
    }
}
