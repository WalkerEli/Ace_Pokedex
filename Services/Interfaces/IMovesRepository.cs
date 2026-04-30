using TeamAceProject.Models.ViewModels.Moves;

namespace TeamAceProject.Services.Interfaces
{
    public interface IMovesRepository
    {
        Task<MoveListPageViewModel> GetMovesPageAsync(int pageNumber, int pageSize, string? query = null);
        Task<MoveDetailViewModel?> GetMoveDetailsAsync(string name);
    }
}
