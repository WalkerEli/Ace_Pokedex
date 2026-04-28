using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Models.ViewModels.Natures;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Controllers
{
    public class NaturesController : Controller
    {
        private readonly INaturesService _naturesService;

        public NaturesController(INaturesService naturesService)
        {
            _naturesService = naturesService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? query = null)
        {
            NatureListViewModel model = await _naturesService.GetAllNaturesAsync(query);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return NotFound();

            NatureDetailViewModel? model = await _naturesService.GetNatureDetailsAsync(name);
            if (model == null)
                return NotFound();

            return View(model);
        }
    }
}
