using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Models.ViewModels.Abilities;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Controllers
{
    public class AbilitiesController : Controller
    {
        private readonly IAbilitiesRepository _abilitiesRepo;

        public AbilitiesController(IAbilitiesRepository AbilitiesRepository)
        {
            _abilitiesRepo = AbilitiesRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 30, string? query = null)
        {
            AbilityListPageViewModel model = await _abilitiesRepo.GetAbilitiesPageAsync(page, pageSize, query);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return NotFound();

            AbilityDetailViewModel? model = await _abilitiesRepo.GetAbilityDetailsAsync(name);
            if (model == null)
                return NotFound();

            return View(model);
        }
    }
}
