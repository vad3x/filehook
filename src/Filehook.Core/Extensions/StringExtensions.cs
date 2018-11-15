using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtensions
    {
        public static string GenerateSlug(this string phrase)
        {
            var str = phrase.RemoveAccent().ToLower();
            // invalid chars
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens
            return str;
        }

        public static string RemoveAccent(this string txt)
        {
            var bytes = Text.Encoding.UTF8.GetBytes(txt);
            return Text.Encoding.ASCII.GetString(bytes);
        }
    }
}
