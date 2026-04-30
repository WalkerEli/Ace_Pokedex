using System.ComponentModel.DataAnnotations;

namespace TeamAceProject.Models.Entities;

public class Comment
{
    public Guid Id { get; set; }

    public Guid PostId { get; set; }

    public Guid UserId { get; set; }

    [Required]
    [StringLength(1000)]
    public string Body { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Post? Post { get; set; }
    public User? User { get; set; }
}
