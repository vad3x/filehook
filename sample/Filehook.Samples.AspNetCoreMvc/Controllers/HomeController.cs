using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public HomeController(IFilehookService filehookService, IMapper mapper)
        {
            _filehookService = filehookService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {
            Article[] articles = _articleStore
                .OrderByDescending(a => a.CreatedAt)
                .ToArray();

            FilehookAttachment[] attachments = await _filehookService
                .GetAttachmentsAsync(articles, cancellationToken: cancellationToken);

            ViewBag.Articles = articles.Select(a =>
                _mapper.Map(a, new ArticleViewModel
                {
                    CoverImage = attachments.FindBlob(a, COVER_IMAGE_NAME),
                    Attachments = attachments.FindBlobs(a, ATTACHMENT_NAME)
                }))
            .ToArray();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AttachmentViewModel viewModel, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                Article[] articles = _articleStore.ToArray();

                FilehookAttachment[] attachments = await _filehookService
                    .GetAttachmentsAsync(articles, cancellationToken: cancellationToken);

                ViewBag.Articles = articles.Select(a =>
                    _mapper.Map(a, new ArticleViewModel
                    {
                        CoverImage = attachments.FindBlob(a, COVER_IMAGE_NAME),
                        Attachments = attachments.FindBlobs(a, ATTACHMENT_NAME)
                    }))
                .ToArray();

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
                using (var stream = viewModel.CoverImageFile.OpenReadStream())
                {
                    var fileInfo = new FilehookFileInfo(
                        viewModel.CoverImageFile.ContentType,
                        viewModel.CoverImageFile.FileName,
                        stream);

                    var a = Stopwatch.StartNew();
                    await _filehookService.SetAttachmentAsync(model, COVER_IMAGE_NAME, fileInfo, cancellationToken: cancellationToken);
                    a.Stop();

                    Console.WriteLine($"======================= {a.Elapsed.TotalSeconds}");
                }
            }

            if (viewModel.AttachmentFile != null)
            {
                using (var stream = viewModel.AttachmentFile.OpenReadStream())
                {
                    var fileInfo = new FilehookFileInfo(
                    viewModel.AttachmentFile.ContentType,
                    viewModel.AttachmentFile.FileName,
                    stream);

                    await _filehookService.AddAttachmentAsync(model, ATTACHMENT_NAME, fileInfo, cancellationToken: cancellationToken);
                }
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

            await _filehookService.PurgeAsync(article);

            _articleStore.Remove(article);

            return RedirectToAction(nameof(Index));
        }
    }
}
