using TeamAceProject.Models.Entities;
using TeamAceProject.Models.ViewModels.Teams;

namespace TeamAceProject.Services.Interfaces
{
    public interface ITeamService
    {
        Task<List<TeamListItemViewModel>> GetAllTeamsAsync();
        Task<TeamDetailsViewModel?> GetTeamByIdAsync(Guid teamId);
        Task<Team> CreateTeamAsync(Team team);
        Task<bool> DeleteTeamAsync(Guid teamId);
        Task<TeamMember?> GetTeamMemberByIdAsync(Guid teamMemberId);
        Task<TeamMember> AddTeamMemberAsync(AddTeamMemberInputModel input);
        Task<bool> RemoveTeamMemberAsync(Guid teamMemberId);
        Task<TeamMemberMove> AddTeamMemberMoveAsync(TeamMemberMove teamMemberMove);
        Task<bool> RemoveTeamMemberMoveAsync(Guid teamMemberMoveId);
    }
}
