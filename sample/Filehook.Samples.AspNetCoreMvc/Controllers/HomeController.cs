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
        private static readonly List<Article> _articleStore = new List<Article>();

        private readonly INewFilehookService _filehookService;

        public HomeController(INewFilehookService filehookService)
        {
            _filehookService = filehookService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Articles = _articleStore.OrderByDescending(a => a.CreatedAt);
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

            var model = _articleStore.FirstOrDefault(x => x.Id == viewModel.Id);

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

                await _filehookService.SetOneAsync(model, "CoverImage", fileInfo, cancellationToken);
            }

            if (viewModel.AttachmentFile != null)
            {
                var fileInfo = new FilehookFileInfo(
                    viewModel.AttachmentFile.ContentType,
                    viewModel.AttachmentFile.FileName,
                    viewModel.AttachmentFile.OpenReadStream());

                await _filehookService.AddManyAsync(model, "Attachment", fileInfo, cancellationToken);
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
