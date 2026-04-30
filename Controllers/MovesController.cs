using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Models.ViewModels.Moves;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Controllers
{
    public class MovesController : Controller
    {
        private readonly IMovesRepository _movesRepo;

        public MovesController(IMovesRepository MovesRepository)
        {
            _movesRepo = MovesRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 24, string? query = null)
        {
            MoveListPageViewModel model = await _movesRepo.GetMovesPageAsync(page, pageSize, query);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return NotFound();

            MoveDetailViewModel? model = await _movesRepo.GetMoveDetailsAsync(name);
            if (model == null)
                return NotFound();

            return View(model);
        }
    }
}
