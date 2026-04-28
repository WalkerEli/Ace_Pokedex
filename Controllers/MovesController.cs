using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Models.ViewModels.Moves;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Controllers
{
    public class MovesController : Controller
    {
        private readonly IMovesService _movesService;

        public MovesController(IMovesService movesService)
        {
            _movesService = movesService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 24, string? query = null)
        {
            MoveListPageViewModel model = await _movesService.GetMovesPageAsync(page, pageSize, query);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return NotFound();

            MoveDetailViewModel? model = await _movesService.GetMoveDetailsAsync(name);
            if (model == null)
                return NotFound();

            return View(model);
        }
    }
}
