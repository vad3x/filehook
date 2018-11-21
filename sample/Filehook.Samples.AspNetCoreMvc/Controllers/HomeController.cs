using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Filehook.Abstractions;
using Filehook.Samples.AspNetCoreMvc.Models;
using Filehook.Samples.AspNetCoreMvc.ViewModels;

using Microsoft.AspNetCore.Mvc;

namespace Filehook.Samples.AspNetCoreMvc.Controllers
{
    public class HomeController : Controller
    {
        private const string COVER_IMAGE_NAME = "CoverImage";
        private const string ATTACHMENT_NAME = "Attachment";

        private static readonly List<Article> _articleStore = new List<Article>();

        private readonly IFilehookService _filehookService;

        public HomeController(IFilehookService filehookService)
        {
            _filehookService = filehookService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {
            Article[] articles = _articleStore
                .OrderByDescending(a => a.CreatedAt)
                .ToArray();

            FilehookAttachment[] attachments = await _filehookService
                .GetAttachmentsAsync(articles, cancellationToken: cancellationToken);

            ViewBag.Articles = articles.Select(a => new ArticleViewModel
            {
                Id = a.Id,
                CreatedAt = a.CreatedAt,
                CoverImage = _filehookService.GetSingleBlob(a, COVER_IMAGE_NAME, attachments),
                Attachments = _filehookService.GetManyBlobs(a, ATTACHMENT_NAME, attachments)
            })
            .ToArray();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AttachmentViewModel viewModel, CancellationToken cancellationToken = default)
        {
            ViewBag.Articles = _articleStore;

            if (!ModelState.IsValid)
            {
                return View();
            }

            Article model = _articleStore.FirstOrDefault(x => x.Id == viewModel.Id);

            if (model == null)
            {
                model = new Article
                {
                    Id = viewModel.Id,
                    CreatedAt = DateTime.Now
                };

                _articleStore.Add(model);
            }

            if (viewModel.CoverImageFile != null)
            {
                var fileInfo = new FilehookFileInfo(
                    viewModel.CoverImageFile.ContentType,
                    viewModel.CoverImageFile.FileName,
                    viewModel.CoverImageFile.OpenReadStream());

                await _filehookService.SetOneAsync(model, COVER_IMAGE_NAME, fileInfo, cancellationToken);
            }

            if (viewModel.AttachmentFile != null)
            {
                var fileInfo = new FilehookFileInfo(
                    viewModel.AttachmentFile.ContentType,
                    viewModel.AttachmentFile.FileName,
                    viewModel.AttachmentFile.OpenReadStream());

                await _filehookService.AddManyAsync(model, ATTACHMENT_NAME, fileInfo, cancellationToken);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var article = _articleStore.FirstOrDefault(a => a.Id == id);
            if (article == null)
            {
                return View();
            }

            //if (article.CoverImageFileName != null)
            //{
            //    await _filehookService.RemoveAsync(article, a => a.CoverImageFileName);
            //}

            //if (article.AttachmentFileName != null)
            //{
            //    await _filehookService.RemoveAsync(article, a => a.AttachmentFileName);
            //}

            //_articleStore.Remove(article);

            return RedirectToAction(nameof(Index));
        }
    }
}
