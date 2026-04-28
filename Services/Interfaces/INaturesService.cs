using TeamAceProject.Models.ViewModels.Natures;

namespace TeamAceProject.Services.Interfaces
{
    public interface INaturesService
    {
        Task<NatureListViewModel> GetAllNaturesAsync(string? query = null);
        Task<NatureDetailViewModel?> GetNatureDetailsAsync(string name);
    }
}
