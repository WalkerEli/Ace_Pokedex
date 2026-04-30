using System.ComponentModel.DataAnnotations;

namespace TeamAceProject.Models.Entities;

public class User
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? FavoritePokemonId { get; set; }

    [StringLength(100)]
    public string? FavoritePokemonName { get; set; }

    [StringLength(500)]
    public string? Bio { get; set; }

    public ICollection<Team> Teams { get; set; } = new List<Team>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
}
