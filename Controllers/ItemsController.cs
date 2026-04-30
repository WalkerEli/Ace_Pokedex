using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Models.ViewModels.Items;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Controllers
{
    public class ItemsController : Controller
    {
        private readonly IItemRepository _itemRepo;

        public ItemsController(IItemRepository ItemRepository)
        {
            _itemRepo = ItemRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 24, string? query = null)
        {
            ItemListPageViewModel model = await _itemRepo.GetHeldItemsPageAsync(page, pageSize, query);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return NotFound();

            ItemDetailViewModel? model = await _itemRepo.GetItemDetailsAsync(name);
            if (model == null)
                return NotFound();

            return View(model);
        }
    }
}
