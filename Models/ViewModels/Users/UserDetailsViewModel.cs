namespace TeamAceProject.Models.ViewModels.Users
{
    public class UserDetailsViewModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? FavoritePokemonId { get; set; }
        public string? FavoritePokemonName { get; set; }
        public int TeamCount { get; set; }
        public int PostCount { get; set; }
    }
}
