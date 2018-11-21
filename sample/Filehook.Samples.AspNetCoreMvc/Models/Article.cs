using System;
using Filehook.DataAnnotations.Abstractions;

namespace Filehook.Samples.AspNetCoreMvc.Models
{
    [HasName("MyArticle")]
    public class Article
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
