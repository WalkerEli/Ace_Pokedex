using System.ComponentModel.DataAnnotations;

namespace TeamAceProject.Models.Entities;

public class TeamMemberMove
{
    public int Id { get; set; }

    public int TeamMemberId { get; set; }

    [Range(1, 4)]
    public int MoveSlot { get; set; }

    [Required]
    [StringLength(100)]
    public string MoveName { get; set; } = string.Empty;

    public TeamMember TeamMember { get; set; } = null!;
}
