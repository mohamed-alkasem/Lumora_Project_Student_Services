using System.ComponentModel.DataAnnotations;

namespace Lumora.Web.Models.Account;

public class ProfileViewModel
{
    [Required(ErrorMessage = "RequiredField")]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Phone(ErrorMessage = "RequiredField")]
    [StringLength(30)]
    public string? PhoneNumber { get; set; }

    /// Shown read-only; the account identifier never changes from this screen.
    public string Email { get; set; } = string.Empty;
}

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "RequiredField")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "RequiredField")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "PasswordTooShort")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "RequiredField")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "PasswordMismatch")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
