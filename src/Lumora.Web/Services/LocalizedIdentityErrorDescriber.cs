using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Lumora.Web.Services;

/// Translates the Identity error messages users actually hit (duplicate email,
/// password composition rules). Anything not overridden falls back to the
/// built-in English descriptions.
public class LocalizedIdentityErrorDescriber : IdentityErrorDescriber
{
    private readonly IStringLocalizer<SharedResources> _localizer;

    public LocalizedIdentityErrorDescriber(IStringLocalizer<SharedResources> localizer)
    {
        _localizer = localizer;
    }

    public override IdentityError DuplicateEmail(string email) =>
        new() { Code = nameof(DuplicateEmail), Description = _localizer["DuplicateEmail"] };

    // UserName == Email in this app, so a duplicate user name is the same situation.
    public override IdentityError DuplicateUserName(string userName) =>
        new() { Code = nameof(DuplicateUserName), Description = _localizer["DuplicateEmail"] };

    public override IdentityError InvalidEmail(string? email) =>
        new() { Code = nameof(InvalidEmail), Description = _localizer["InvalidEmail"] };

    public override IdentityError PasswordTooShort(int length) =>
        new() { Code = nameof(PasswordTooShort), Description = _localizer["PasswordTooShort"] };

    public override IdentityError PasswordRequiresDigit() =>
        new() { Code = nameof(PasswordRequiresDigit), Description = _localizer["PasswordRequiresDigit"] };

    public override IdentityError PasswordRequiresLower() =>
        new() { Code = nameof(PasswordRequiresLower), Description = _localizer["PasswordRequiresLower"] };

    public override IdentityError PasswordRequiresUpper() =>
        new() { Code = nameof(PasswordRequiresUpper), Description = _localizer["PasswordRequiresUpper"] };

    public override IdentityError PasswordMismatch() =>
        new() { Code = nameof(PasswordMismatch), Description = _localizer["PasswordIncorrect"] };
}
