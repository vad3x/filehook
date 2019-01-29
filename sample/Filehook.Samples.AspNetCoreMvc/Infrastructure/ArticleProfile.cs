
using AutoMapper;

using Filehook.Samples.AspNetCoreMvc.Models;

namespace Filehook.Samples.AspNetCoreMvc.Infrastructure
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<Article, ViewModels.ArticleViewModel>();
        }
    }
}
