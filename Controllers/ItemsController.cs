using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Models.ViewModels.Items;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Controllers
{
    public class ItemsController : Controller
    {
        private readonly IItemService _itemService;

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 24, string? query = null)
        {
            ItemListPageViewModel model = await _itemService.GetHeldItemsPageAsync(page, pageSize, query);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return NotFound();

            ItemDetailViewModel? model = await _itemService.GetItemDetailsAsync(name);
            if (model == null)
                return NotFound();

            return View(model);
        }
    }
}
