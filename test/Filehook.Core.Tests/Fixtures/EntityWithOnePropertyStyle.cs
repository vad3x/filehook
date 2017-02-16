namespace Filehook.Core.Tests.Fixtures
{
    public class EntityWithOnePropertyStyle
    {
        [HasFileStyle("regular")]
        public string FileName { get; set; }
    }
}
