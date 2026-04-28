using TeamAceProject.Models.ViewModels.Abilities;

namespace TeamAceProject.Services.Interfaces
{
    public interface IAbilitiesService
    {
        Task<AbilityListPageViewModel> GetAbilitiesPageAsync(int pageNumber, int pageSize, string? query = null);
        Task<AbilityDetailViewModel?> GetAbilityDetailsAsync(string name);
    }
}
