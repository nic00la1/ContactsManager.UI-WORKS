using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.DTO;
using ContactsManager.Core.Enums;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace ContactsManager.UI.Controllers;

[Route("[controller]/[action]")]
[AllowAnonymous]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public AccountController(UserManager<ApplicationUser> userManager,
                             SignInManager<ApplicationUser> signInManager,
                             RoleManager<ApplicationRole> roleManager
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDTO registerDto)
    {
        // Check for validation errors
        if (ModelState.IsValid == false)
        {
            ViewBag.Errors = ModelState.Values.SelectMany(e => e.Errors)
                .Select(t => t.ErrorMessage);
            return View(registerDto);
        }

        ApplicationUser user = new()
        {
            Email = registerDto.Email,
            PhoneNumber = registerDto.Phone,
            UserName = registerDto.Email,
            PersonName = registerDto.PersonName
        };

        IdentityResult result =
            await _userManager.CreateAsync(user, registerDto.Password);

        if (result.Succeeded)
        {
            // Check the status of radio buttons
            if (registerDto.UserType == UserTypeOptions.Admin)
            {
                // Create 'Admin' role
                if (await _roleManager.FindByNameAsync(UserTypeOptions.Admin
                        .ToString()) is null)
                {
                    ApplicationRole applicationRole = new()
                    {
                        Name = UserTypeOptions.Admin.ToString()
                    };

                    await _roleManager.CreateAsync(applicationRole);
                }

                // Add the new user into 'Admin' role
                await _userManager.AddToRoleAsync(user, UserTypeOptions.Admin
                    .ToString());
            } else
            {
                // Create 'User' role
                if (await _roleManager.FindByNameAsync(UserTypeOptions.User
                        .ToString()) is null)
                {
                    ApplicationRole applicationRole = new()
                    {
                        Name = UserTypeOptions.User.ToString()
                    };

                    await _roleManager.CreateAsync(applicationRole);
                }

                // Add the new user into 'User' role
                await _userManager.AddToRoleAsync(user, UserTypeOptions.User
                    .ToString());
            }

            // Sign in
            await _signInManager.SignInAsync(user, true);

            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        } else
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("Register", error.Description);

        return View(registerDto);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDTO loginDto, string? ReturnUrl)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Errors = ModelState.Values.SelectMany(e => e.Errors)
                .Select(t => t.ErrorMessage);
            return View(loginDto);
        }

        SignInResult result = await _signInManager.PasswordSignInAsync(
            loginDto.Email, loginDto.Password, false, false);

        if (result.Succeeded)
        {
            // Admin
            ApplicationUser user = await
                _userManager.FindByEmailAsync(loginDto.Email);

            if (user != null)
                if (await _userManager.IsInRoleAsync(user,
                        UserTypeOptions.Admin.ToString()))
                    return RedirectToAction("Index", "Home",
                        new { area = "Admin" });

            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                return LocalRedirect(ReturnUrl);
            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }

        ModelState.AddModelError("Login", "Invalid email or password.");

        return View(loginDto);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
    {
        ApplicationUser user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Json(true);
        return Json($"Email {email} is already registered.");
    }
}
