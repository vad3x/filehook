using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Filehook.Abstractions;
using Filehook.Proccessors.Image.Abstractions;
using Filehook.Samples.AspNetCoreMvc.Models;
using Filehook.Samples.AspNetCoreMvc.ViewModels;

using Microsoft.AspNetCore.Http;
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

            var model = new Article
            {
                Id = viewModel.Id,
                CreatedAt = DateTime.Now
            };

            //if (viewModel.AttachmentFile != null)
            //{
            //    var bytes = await viewModel.AttachmentFile.GetBytesAsync();

            //    var fileExtention = Path.GetExtension(viewModel.AttachmentFile.FileName)?.Trim('.');
            //    if (!_filehookService.CanProccess(fileExtention, bytes))
            //    {
            //        ModelState.AddModelError(nameof(viewModel.AttachmentFile), "Could not be proccessed");
            //        return View();
            //    }

            //    var results = await _filehookService.SaveAsync(model, a => a.AttachmentFileName, viewModel.AttachmentFile.FileName, bytes);
            //}

            //if (viewModel.CoverImageFile != null)
            //{
            //    var bytes = await viewModel.CoverImageFile.GetBytesAsync();

            //    var fileExtention = Path.GetExtension(viewModel.CoverImageFile.FileName)?.Trim('.');
            //    if (!_filehookService.CanProccess(fileExtention, bytes))
            //    {
            //        ModelState.AddModelError(nameof(viewModel.CoverImageFile), "Could not be proccessed");
            //        return View();
            //    }

            //    var results = await _filehookService.SaveAsync(model, a => a.CoverImageFileName, viewModel.CoverImageFile.FileName, bytes);
            //    if (results["thumb"].ProccessingMeta is ImageProccessingResultMeta data)
            //    {
            //        model.CoverImageAspectRatio = (float)data.Width / data.Height;
            //    }
            //}

            if (viewModel.CoverImageFile != null)
            {
                var fileInfo = new FilehookFileInfo(
                    viewModel.CoverImageFile.ContentType,
                    viewModel.CoverImageFile.FileName,
                    viewModel.CoverImageFile.OpenReadStream());

                var result = await _filehookService.SaveAsync(model, "CoverImage", fileInfo, cancellationToken);
            }

            _articleStore.Add(model);

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
