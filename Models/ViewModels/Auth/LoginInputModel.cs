using System.ComponentModel.DataAnnotations;

namespace TeamAceProject.Models.ViewModels.Auth;

public class LoginInputModel
{
    [Required]
    [Display(Name = "username or email")]
    public string UsernameOrEmail { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
