using System.ComponentModel.DataAnnotations;

namespace TeamAceProject.Models.ViewModels.Teams
{
    public class EditTeamMemberInputModel
    {
        public Guid TeamMemberId { get; set; }
        public Guid TeamId { get; set; }

        [Required]
        [StringLength(100)]
        public string PokemonName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? AbilityName { get; set; }

        [StringLength(100)]
        public string? NatureName { get; set; }

        [StringLength(100)]
        public string? HeldItemName { get; set; }

        [StringLength(100)]
        public string? Move1 { get; set; }

        [StringLength(100)]
        public string? Move2 { get; set; }

        [StringLength(100)]
        public string? Move3 { get; set; }

        [StringLength(100)]
        public string? Move4 { get; set; }
    }
}
