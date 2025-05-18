using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using TouristAgency.Models;

public class RegisterAgencyModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RegisterAgencyModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    [BindProperty]
    public RegisterAgencyViewModel Input { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var user = new ApplicationUser
        {
            UserName = Input.UserName,
            Email = Input.Email,
            PhoneNumber = Input.PhoneNumber,
            Address = Input.Address,
            Description = Input.Description,
            IsApproved = false
        };

        var result = await _userManager.CreateAsync(user, Input.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "TravelAgency");
            // НЕ влиза в системата автоматично:
            return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return Page();
    }
}
