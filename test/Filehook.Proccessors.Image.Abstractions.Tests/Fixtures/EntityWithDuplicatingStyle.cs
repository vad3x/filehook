namespace Filehook.Proccessors.Image.Abstractions.Tests.Fixtures
{
    public class EntityWithDuplicatingStyleName
    {
        [HasImageStyle("regular", 100, 100)]
        [HasImageStyle("regular", 200, 100)]
        public string ImageName { get; set; }
    }
}
