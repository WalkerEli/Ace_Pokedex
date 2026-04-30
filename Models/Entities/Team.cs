using System.ComponentModel.DataAnnotations;

namespace TeamAceProject.Models.Entities;

public class Team
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public bool IsPublic { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}
