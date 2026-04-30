using System.ComponentModel.DataAnnotations;

namespace TeamAceProject.Models.Entities;

public class Post
{
    public int Id { get; set; }

    public int TeamId { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [StringLength(1000)]
    public string Caption { get; set; } = string.Empty;

    public Team? Team { get; set; }
    public User? User { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
}
