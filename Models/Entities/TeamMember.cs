using System.ComponentModel.DataAnnotations;

namespace TeamAceProject.Models.Entities;

public class TeamMember
{
    public Guid Id { get; set; }

    public Guid TeamId { get; set; }

    [Range(1, 6)]
    public int SlotIndex { get; set; }

    public int PokemonId { get; set; }

    [Required]
    [StringLength(100)]
    public string PokemonName { get; set; } = string.Empty;

    [StringLength(250)]
    public string? PokemonSpriteUrl { get; set; }

    [StringLength(100)]
    public string? PokemonTypes { get; set; }

    [StringLength(100)]
    public string? AbilityName { get; set; }

    [StringLength(100)]
    public string? NatureName { get; set; }

    [StringLength(100)]
    public string? HeldItemName { get; set; }

    public Team Team { get; set; } = null!;
    public ICollection<TeamMemberMove> TeamMemberMoves { get; set; } = new List<TeamMemberMove>();
}
