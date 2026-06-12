using System.ComponentModel.DataAnnotations;

namespace Logistics.IdentityServer.Pages.Account.Login;

public class InputModel
{
    // No [EmailAddress]: seeded/test accounts may sign in with a plain username
    // (e.g. "1230"), not only an email. Lookup falls back to username in the handler.
    [Required]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    public bool RememberLogin { get; set; }

    public string ReturnUrl { get; set; } = "/";

    public string Button { get; set; }
}
