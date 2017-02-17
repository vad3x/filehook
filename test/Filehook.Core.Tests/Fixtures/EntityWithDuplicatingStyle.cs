using Filehook.DataAnnotations;

namespace Filehook.Core.Tests.Fixtures
{
    public class EntityWithDuplicatingStyleName
    {
        [HasFileStyle("regular")]
        [HasFileStyle("regular")]
        public string FileName { get; set; }
    }
}
