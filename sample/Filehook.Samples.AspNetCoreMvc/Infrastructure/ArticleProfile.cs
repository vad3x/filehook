using System;
using System.Linq;

using AutoMapper;

using Filehook.Abstractions;
using Filehook.Samples.AspNetCoreMvc.Models;

namespace Filehook.Samples.AspNetCoreMvc.Infrastructure
{
    //public class ArticleProfile : Profile
    //{
    //    public ArticleProfile()
    //    {
    //        CreateMap<Article, ViewModels.ArticleViewModel>()
    //            .ConvertUsing<BlobTypeConverter<Article, ViewModels.ArticleViewModel>>()
    //            //.ForMember(dest => dest.CoverImage, opt => opt.ResolveUsing<FilehookBlobMemberValueResolver<Article>, Article>())
    //            //.ForMember(dest => dest.CoverImage, opt => opt.ResolveUsing<FilehookBlobValueResolver>())
    //            ;
    //    }
    //}

    //public class BlobTypeConverter<TSource, TDestination> : ITypeConverter<TSource, TDestination>
    //{
    //    private readonly INewFilehookService _filehookService;

    //    public BlobTypeConverter(INewFilehookService filehookService)
    //    {
    //        _filehookService = filehookService ?? throw new ArgumentNullException(nameof(filehookService));
    //    }

    //    public TDestination Convert(TSource source, TDestination destination, ResolutionContext context)
    //    {
    //        var blobProperies = typeof(TSource).GetProperties().Where(p => p.PropertyType == typeof(FilehookBlob));
    //        foreach (var property in blobProperies)
    //        {
    //            var propertyName = property.Name;

    //            _filehookService.GetBlobsAsync(source, propertyName);
    //        }

    //        return destination;
    //    }
    //}
}
