using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Models.ViewModels.Natures;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Controllers
{
    public class NaturesController : Controller
    {
        private readonly INaturesRepository _naturesRepo;

        public NaturesController(INaturesRepository NaturesRepository)
        {
            _naturesRepo = NaturesRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? query = null)
        {
            NatureListViewModel model = await _naturesRepo.GetAllNaturesAsync(query);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return NotFound();

            NatureDetailViewModel? model = await _naturesRepo.GetNatureDetailsAsync(name);
            if (model == null)
                return NotFound();

            return View(model);
        }
    }
}
