using TeamAceProject.Models.Entities;
using TeamAceProject.Models.ViewModels.Teams;

namespace TeamAceProject.Services.Interfaces
{
    public interface ITeamRepository
    {
        Task<List<TeamListItemViewModel>> GetAllTeamsAsync();
        Task<List<TeamListItemViewModel>> GetTeamsByUserAsync(int userId);
        Task<TeamDetailsViewModel?> GetTeamByIdAsync(int teamId);
        Task<int?> GetTeamOwnerIdAsync(int teamId);
        Task<List<TeamOptionViewModel>> GetUserTeamOptionsAsync(int userId);
        Task<Team> CreateTeamAsync(Team team);
        Task<bool> RenameTeamAsync(int teamId, string newName);
        Task<bool> DeleteTeamAsync(int teamId);
        Task<TeamMember?> GetTeamMemberByIdAsync(int teamMemberId);
        Task<TeamMember> AddTeamMemberAsync(AddTeamMemberInputModel input);
        Task<TeamMember> EditTeamMemberAsync(EditTeamMemberInputModel input);
        Task<bool> RemoveTeamMemberAsync(int teamMemberId);
        Task<TeamMemberMove> AddTeamMemberMoveAsync(TeamMemberMove teamMemberMove);
        Task<bool> RemoveTeamMemberMoveAsync(int teamMemberMoveId);
    }
}
