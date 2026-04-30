using TeamAceProject.Models.Enums;

namespace TeamAceProject.Models.Entities;

public class Reaction
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public ReactionType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Post Post { get; set; } = null!;
    public User User { get; set; } = null!;
}
