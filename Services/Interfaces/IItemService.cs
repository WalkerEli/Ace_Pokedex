using TeamAceProject.Models.ViewModels.Items;

namespace TeamAceProject.Services.Interfaces
{
    public interface IItemService
    {
        Task<ItemListPageViewModel> GetHeldItemsPageAsync(int pageNumber, int pageSize, string? query = null);
        Task<ItemDetailViewModel?> GetItemDetailsAsync(string name);
    }
}
