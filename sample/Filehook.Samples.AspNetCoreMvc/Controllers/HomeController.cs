using Filehook.Abstractions;
using Filehook.Samples.AspNetCoreMvc.Models;
using Filehook.Samples.AspNetCoreMvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Filehook.Proccessors.Image.Abstractions;
using System.Linq;
using System;

namespace Filehook.Samples.AspNetCoreMvc.Controllers
{
    public class HomeController : Controller
    {
        private static readonly List<Article> _articleStore = new List<Article>();

        private readonly IFilehookService _filehookService;

        public HomeController(IFilehookService filehookService)
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
        public async Task<IActionResult> Index(AttachmentViewModel viewModel)
        {
            ViewBag.Articles = _articleStore;

            if (!ModelState.IsValid)
            {
                return View();
            }

            var model = new Article
            {
                Id = viewModel.Id,
                CoverImageFileName = viewModel.CoverImageFile?.FileName,
                AttachmentFileName = viewModel.AttachmentFile?.FileName,
                CreatedAt = DateTime.Now
            };

            if (viewModel.AttachmentFile != null)
            {
                using (var memoryStream = new MemoryStream())
                using (var sourceStream = viewModel.AttachmentFile.OpenReadStream())
                {
                    sourceStream.CopyTo(memoryStream);
                    var bytes = memoryStream.ToArray();

                    var fileExtention = Path.GetExtension(viewModel.AttachmentFile.FileName)?.Trim('.');
                    if (!_filehookService.CanProccess(fileExtention, bytes))
                    {
                        ModelState.AddModelError(nameof(viewModel.AttachmentFile), "Could not be proccessed");
                        return View();
                    }

                    var results = await _filehookService.SaveAsync(model, a => a.AttachmentFileName, bytes, model.Id.ToString());
                }
            }

            if (viewModel.CoverImageFile != null)
            {
                using (var memoryStream = new MemoryStream())
                using (var sourceStream = viewModel.CoverImageFile.OpenReadStream())
                {
                    sourceStream.CopyTo(memoryStream);
                    var bytes = memoryStream.ToArray();

                    var fileExtention = Path.GetExtension(viewModel.CoverImageFile.FileName)?.Trim('.');
                    if (!_filehookService.CanProccess(fileExtention, bytes))
                    {
                        ModelState.AddModelError(nameof(viewModel.CoverImageFile), "Could not be proccessed");
                        return View();
                    }

                    var results = await _filehookService.SaveAsync(model, a => a.CoverImageFileName, bytes, model.Id.ToString());
                    if (results["thumb"].ProccessingMeta is ImageProccessingResultMeta data)
                    {
                        model.CoverImageAspectRatio = (float)data.Width / data.Height;
                    }
                }
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

            if (article.CoverImageFileName != null)
            {
                await _filehookService.RemoveAsync(article, a => a.CoverImageFileName, article.Id.ToString());
            }

            if (article.AttachmentFileName != null)
            {
                await _filehookService.RemoveAsync(article, a => a.AttachmentFileName, article.Id.ToString());
            }

            _articleStore.Remove(article);

            return RedirectToAction(nameof(Index));
        }
    }
}
