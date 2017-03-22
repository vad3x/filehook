using Filehook.Abstractions;
using Filehook.Samples.AspNetCoreMvc.Models;
using Filehook.Samples.AspNetCoreMvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Filehook.Proccessors.Image.Abstractions;

namespace Filehook.Samples.AspNetCoreMvc.Controllers
{
    public class HomeController : Controller
    {
        private static readonly Stack<Article> _articleStore = new Stack<Article>();

        private readonly IFilehookService _filehookService;

        public HomeController(IFilehookService filehookService)
        {
            _filehookService = filehookService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Articles = _articleStore;
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
                AttachmentFileName = viewModel.AttachmentFile?.FileName
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

            _articleStore.Push(model);

            return RedirectToAction(nameof(Index));
        }
    }
}
