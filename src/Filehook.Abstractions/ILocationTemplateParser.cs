namespace Filehook.Abstractions
{
    public interface ILocationTemplateParser
    {
         string Parse(
             string className = null,
             string attachmentName = null,
             string attachmentId = null,
             string style = null,
             string filename = null,
             string locationTemplate = null);

        string SetBase(string locationTemplate, string baseLocation);
    }
}
