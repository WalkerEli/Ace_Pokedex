using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Infrastructure;
using TeamAceProject.Models.Entities;
using TeamAceProject.Models.ViewModels.Teams;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Controllers
{
    // Manages team creation, member editing, and deletion
    public class TeamsController : Controller
    {
        private readonly ITeamRepository _teamRepo;

        public TeamsController(ITeamRepository DbTeamRepository)
        {
            _teamRepo = DbTeamRepository;
        }


        // Returns the current user's teams as JSON for the team dropdown in post forms
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UserTeams()
        {
            int? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
                return Json(new List<object>());

            var teams = await _teamRepo.GetUserTeamOptionsAsync(currentUserId.Value);
            return Json(teams);
        }

        // Lists the current user's teams if logged in, otherwise shows all teams
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int? currentUserId = User.GetCurrentUserId();
            var teams = currentUserId.HasValue
                ? await _teamRepo.GetTeamsByUserAsync(currentUserId.Value)
                : await _teamRepo.GetAllTeamsAsync();
            return View(teams);
        }

        // Shows a single team with all its members
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var team = await _teamRepo.GetTeamByIdAsync(id);

            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // Shows the create team form
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            int? currentUserId = User.GetCurrentUserId();

            if (!currentUserId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new Team
            {
                IsPublic = true
            });
        }

        // Saves a new team and redirects to its details page
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Team team)
        {
            int? currentUserId = User.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            team.UserId = currentUserId.Value;
            ModelState.Remove(nameof(Team.UserId));

            if (!ModelState.IsValid)
                return View(team);

            Team createdTeam = await _teamRepo.CreateTeamAsync(team);
            return RedirectToAction(nameof(Details), new { id = createdTeam.Id });
        }

        // Validates and adds a new Pokemon to the team after confirming ownership
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMember(AddTeamMemberInputModel input)
        {
            int? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            var team = await _teamRepo.GetTeamOwnerIdAsync(input.TeamId);
            if (team == null || team != currentUserId.Value)
                return Forbid();

            if (!ModelState.IsValid)
            {
                TempData["TeamError"] = "Please fix the member form and try again.";
                return RedirectToAction(nameof(Details), new { id = input.TeamId });
            }

            try
            {
                await _teamRepo.AddTeamMemberAsync(input);
                TempData["TeamSuccess"] = "Pokemon added to the team.";
            }
            catch (Exception ex)
            {
                TempData["TeamError"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id = input.TeamId });
        }

        // Updates a team member's Pokemon, ability, nature, held item, and moves
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMember(EditTeamMemberInputModel input)
        {
            int? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            var ownerId = await _teamRepo.GetTeamOwnerIdAsync(input.TeamId);
            if (ownerId == null || ownerId != currentUserId.Value)
                return Forbid();

            if (!ModelState.IsValid)
            {
                TempData["TeamError"] = "Please fix the form and try again.";
                return RedirectToAction(nameof(Details), new { id = input.TeamId });
            }

            try
            {
                await _teamRepo.EditTeamMemberAsync(input);
                TempData["TeamSuccess"] = "Pokemon updated.";
            }
            catch (Exception ex)
            {
                TempData["TeamError"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id = input.TeamId });
        }

        // Removes a Pokemon slot from the team after confirming ownership
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveMember(int teamId, int teamMemberId)
        {
            int? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            var ownerId = await _teamRepo.GetTeamOwnerIdAsync(teamId);
            if (ownerId == null || ownerId != currentUserId.Value)
                return Forbid();

            bool removed = await _teamRepo.RemoveTeamMemberAsync(teamMemberId);

            TempData[removed ? "TeamSuccess" : "TeamError"] =
                removed ? "Pokemon removed from the team." : "That team member could not be removed.";

            return RedirectToAction(nameof(Details), new { id = teamId });
        }

        // Renames a team after confirming ownership
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rename(int teamId, string name)
        {
            int? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            var ownerId = await _teamRepo.GetTeamOwnerIdAsync(teamId);
            if (ownerId == null || ownerId != currentUserId.Value)
                return Forbid();

            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["TeamError"] = "Team name cannot be empty.";
                return RedirectToAction(nameof(Details), new { id = teamId });
            }

            await _teamRepo.RenameTeamAsync(teamId, name);
            TempData["TeamSuccess"] = "Team name updated.";
            return RedirectToAction(nameof(Details), new { id = teamId });
        }

        // Shows a confirmation page before deleting the team
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var team = await _teamRepo.GetTeamByIdAsync(id);
            if (team == null) return NotFound();
            return View(team);
        }

        // Deletes the team and all its members after confirming ownership
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            var ownerId = await _teamRepo.GetTeamOwnerIdAsync(id);
            if (ownerId == null || ownerId != currentUserId.Value)
                return Forbid();

            bool deleted = await _teamRepo.DeleteTeamAsync(id);

            if (!deleted)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}