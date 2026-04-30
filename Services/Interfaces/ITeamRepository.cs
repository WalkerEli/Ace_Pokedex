using TeamAceProject.Models.Entities;
using TeamAceProject.Models.ViewModels.Teams;

namespace TeamAceProject.Services.Interfaces
{
    public interface ITeamRepository
    {
        Task<List<TeamListItemViewModel>> GetAllTeamsAsync();
        Task<List<TeamListItemViewModel>> GetTeamsByUserAsync(Guid userId);
        Task<TeamDetailsViewModel?> GetTeamByIdAsync(Guid teamId);
        Task<Guid?> GetTeamOwnerIdAsync(Guid teamId);
        Task<List<TeamOptionViewModel>> GetUserTeamOptionsAsync(Guid userId);
        Task<Team> CreateTeamAsync(Team team);
        Task<bool> DeleteTeamAsync(Guid teamId);
        Task<TeamMember?> GetTeamMemberByIdAsync(Guid teamMemberId);
        Task<TeamMember> AddTeamMemberAsync(AddTeamMemberInputModel input);
        Task<TeamMember> EditTeamMemberAsync(EditTeamMemberInputModel input);
        Task<bool> RemoveTeamMemberAsync(Guid teamMemberId);
        Task<TeamMemberMove> AddTeamMemberMoveAsync(TeamMemberMove teamMemberMove);
        Task<bool> RemoveTeamMemberMoveAsync(Guid teamMemberMoveId);
    }
}
