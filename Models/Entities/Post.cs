using System.ComponentModel.DataAnnotations;

namespace TeamAceProject.Models.Entities;

public class Post
{
    public Guid Id { get; set; }

    public Guid TeamId { get; set; }

    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(1000)]
    public string Caption { get; set; } = string.Empty;

    public Team? Team { get; set; }
    public User? User { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
}
