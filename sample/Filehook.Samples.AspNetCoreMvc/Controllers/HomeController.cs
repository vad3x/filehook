using Filehook.Abstractions;
using Filehook.Samples.AspNetCoreMvc.Models;
using Filehook.Samples.AspNetCoreMvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace Filehook.Samples.AspNetCoreMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFilehookService _filehookService;

        public HomeController(IFilehookService filehookService)
        {
            _filehookService = filehookService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AttachmentViewModel viewModel)
        {
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

                    var urls = await _filehookService.SaveAsync(model, a => a.AttachmentFileName, memoryStream.ToArray(), model.Id.ToString());

                    ViewBag.AttachmentUrls = urls;
                }
            }

            if (viewModel.CoverImageFile != null)
            {
                using (var memoryStream = new MemoryStream())
                using (var sourceStream = viewModel.CoverImageFile.OpenReadStream())
                {
                    sourceStream.CopyTo(memoryStream);

                    var urls = await _filehookService.SaveAsync(model, a => a.CoverImageFileName, memoryStream.ToArray(), model.Id.ToString());

                    ViewBag.Urls = urls;
                }
            }

            return View();
        }
    }
}
