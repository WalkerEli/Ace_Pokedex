using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Infrastructure;
using TeamAceProject.Models.Entities;
using TeamAceProject.Models.ViewModels.Teams;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Controllers
{
    // Manages team creation, member editing, and deletion
    public class TeamsController(ITeamService teamService) : Controller
    {

        // Returns the current user's teams as JSON for the team dropdown in post forms
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UserTeams()
        {
            Guid? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
                return Json(new List<object>());

            var teams = await teamService.GetUserTeamOptionsAsync(currentUserId.Value);
            return Json(teams);
        }

        // Lists the current user's teams if logged in, otherwise shows all teams
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            Guid? currentUserId = User.GetCurrentUserId();
            var teams = currentUserId.HasValue
                ? await teamService.GetTeamsByUserAsync(currentUserId.Value)
                : await teamService.GetAllTeamsAsync();
            return View(teams);
        }

        // Shows a single team with all its members
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var team = await teamService.GetTeamByIdAsync(id);

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
            Guid? currentUserId = User.GetCurrentUserId();

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
            Guid? currentUserId = User.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            team.UserId = currentUserId.Value;
            ModelState.Remove(nameof(Team.UserId));

            if (!ModelState.IsValid)
                return View(team);

            Team createdTeam = await teamService.CreateTeamAsync(team);
            return RedirectToAction(nameof(Details), new { id = createdTeam.Id });
        }

        // Validates and adds a new Pokemon to the team after confirming ownership
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMember(AddTeamMemberInputModel input)
        {
            Guid? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            var team = await teamService.GetTeamOwnerIdAsync(input.TeamId);
            if (team == null || team != currentUserId.Value)
                return Forbid();

            if (!ModelState.IsValid)
            {
                TempData["TeamError"] = "Please fix the member form and try again.";
                return RedirectToAction(nameof(Details), new { id = input.TeamId });
            }

            try
            {
                await teamService.AddTeamMemberAsync(input);
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
            Guid? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            var ownerId = await teamService.GetTeamOwnerIdAsync(input.TeamId);
            if (ownerId == null || ownerId != currentUserId.Value)
                return Forbid();

            if (!ModelState.IsValid)
            {
                TempData["TeamError"] = "Please fix the form and try again.";
                return RedirectToAction(nameof(Details), new { id = input.TeamId });
            }

            try
            {
                await teamService.EditTeamMemberAsync(input);
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
        public async Task<IActionResult> RemoveMember(Guid teamId, Guid teamMemberId)
        {
            Guid? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            var ownerId = await teamService.GetTeamOwnerIdAsync(teamId);
            if (ownerId == null || ownerId != currentUserId.Value)
                return Forbid();

            bool removed = await teamService.RemoveTeamMemberAsync(teamMemberId);

            TempData[removed ? "TeamSuccess" : "TeamError"] =
                removed ? "Pokemon removed from the team." : "That team member could not be removed.";

            return RedirectToAction(nameof(Details), new { id = teamId });
        }

        // Deletes the team and all its members after confirming ownership
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            Guid? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
                return RedirectToAction("Login", "Account");

            var ownerId = await teamService.GetTeamOwnerIdAsync(id);
            if (ownerId == null || ownerId != currentUserId.Value)
                return Forbid();

            bool deleted = await teamService.DeleteTeamAsync(id);

            if (!deleted)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}