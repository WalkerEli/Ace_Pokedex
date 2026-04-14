using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamAceProject.Infrastructure;
using TeamAceProject.Models.Entities;
using TeamAceProject.Models.ViewModels.Teams;
using TeamAceProject.Services.Interfaces;

namespace TeamAceProject.Controllers
{
    public class TeamsController : Controller
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var teams = await _teamService.GetAllTeamsAsync();
            return View(teams);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);

            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            Guid? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            Team team = new Team
            {
                UserId = currentUserId.Value,
                IsPublic = true,
            };

            return View(team);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Team team)
        {
            Guid? currentUserId = User.GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            team.UserId = currentUserId.Value;

            if (!ModelState.IsValid)
            {
                return View(team);
            }

            Team createdTeam = await _teamService.CreateTeamAsync(team);
            return RedirectToAction(nameof(Details), new { id = createdTeam.Id });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMember(AddTeamMemberInputModel input)
        {
            if (!ModelState.IsValid)
            {
                TempData["TeamError"] = "Please fix the member form and try again.";
                return RedirectToAction(nameof(Details), new { id = input.TeamId });
            }

            try
            {
                await _teamService.AddTeamMemberAsync(input);
                TempData["TeamSuccess"] = "Pokemon added to the team.";
            }
            catch (Exception ex)
            {
                TempData["TeamError"] = ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id = input.TeamId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveMember(Guid teamId, Guid teamMemberId)
        {
            bool removed = await _teamService.RemoveTeamMemberAsync(teamMemberId);
            TempData[removed ? "TeamSuccess" : "TeamError"] = removed ? "Pokemon removed from the team." : "That team member could not be removed.";
            return RedirectToAction(nameof(Details), new { id = teamId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            bool deleted = await _teamService.DeleteTeamAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
