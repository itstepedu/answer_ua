using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using AnswerUA.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

// namespace AnswerUA.Areas.Identity.Pages.Account
// {
//     public class AuthModel : PageModel
//     {
//         private readonly SignInManager<ApplicationUser> _signInManager;
//         private readonly UserManager<ApplicationUser> _userManager;
//         private readonly IUserStore<ApplicationUser> _userStore;
//         private readonly IUserEmailStore<ApplicationUser> _emailStore;
//         private readonly ILogger<AuthModel> _logger;
//         private readonly IEmailSender _emailSender;

//         public AuthModel(
//             UserManager<ApplicationUser> userManager,
//             IUserStore<ApplicationUser> userStore,
//             SignInManager<ApplicationUser> signInManager,
//             ILogger<AuthModel> logger,
//             IEmailSender emailSender)
//         {
//             _userManager = userManager;
//             _userStore = userStore;
//             _emailStore = GetEmailStore();
//             _signInManager = signInManager;
//             _logger = logger;
//             _emailSender = emailSender;
//             ExternalLogins = new List<AuthenticationScheme>();
//         }

//         [BindProperty(Name = "Login")]
//         public LoginInputModel Login { get; set; }

//         [BindProperty(Name = "Register", SupportsGet = false)]
//         [ValidateNever]
//         public RegisterInputModel Register { get; set; }

//         public IList<AuthenticationScheme> ExternalLogins { get; set; }

//         public string ReturnUrl { get; set; }

//         public class LoginInputModel
//         {
//             [Required]
//             [EmailAddress]
//             public string Email { get; set; }

//             [Required]
//             [DataType(DataType.Password)]
//             public string Password { get; set; }

//             [Display(Name = "Запам'ятати мене")]
//             public bool RememberMe { get; set; }
//         }

//         public class RegisterInputModel
//         {
//             [Required]
//             [EmailAddress]
//             [Display(Name = "Email")]
//             public string Email { get; set; }

//             [Required]
//             [StringLength(100, ErrorMessage = "Пароль має бути від {2} до {1} символів.", MinimumLength = 6)]
//             [DataType(DataType.Password)]
//             [Display(Name = "Пароль")]
//             public string Password { get; set; }

//             [DataType(DataType.Password)]
//             [Display(Name = "Підтвердити пароль")]
//             [Compare("Password", ErrorMessage = "Паролі не збігаються.")]
//             public string ConfirmPassword { get; set; }
//         }

//         public async Task OnGetAsync(string returnUrl = null)
//         {
//             ReturnUrl = returnUrl ?? Url.Content("~/");
//             ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
//             Console.WriteLine("IM HERE");
//             Login ??= new LoginInputModel();
//             Register ??= new RegisterInputModel();
//         }

//         // --- LOGIN FORM ---
//         public async Task<IActionResult> OnPostLoginAsync(string returnUrl = null)
//         {
//             returnUrl ??= Url.Content("~/");
//             ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
//             Console.WriteLine("IM IN POST");

//             foreach (var state in ModelState)
//             {
//                 Console.WriteLine($"{state.Key}: {state.Value.Errors.Count} errors");
//                 foreach (var error in state.Value.Errors)
//                     Console.WriteLine($"   {error.ErrorMessage}");
//             }

//             if (ModelState.IsValid)
//             {
//                 Console.WriteLine("MODEL STATE IS VALID");
//                 var result = await _signInManager.PasswordSignInAsync(
//                     Login.Email, Login.Password, Login.RememberMe, lockoutOnFailure: false);

//                 Console.WriteLine("IM STILL HERE");
//                 if (result.Succeeded)
//                 {
//                     _logger.LogInformation("User logged in.");
//                     return LocalRedirect(returnUrl);
//                 }

//                 ModelState.AddModelError(string.Empty, "Невдала спроба входу.");
//             }
//             else
//             {
//                 Console.WriteLine("MODEL STATE IS NOT VALID");
//             }

//             return Page();
//         }

//         // --- REGISTER FORM ---
//         public async Task<IActionResult> OnPostRegisterAsync(string returnUrl = null)
//         {
//             returnUrl ??= Url.Content("~/");
//             ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

//             if (ModelState.IsValid)
//             {
//                 var user = new ApplicationUser();
//                 await _userStore.SetUserNameAsync(user, Register.Email, CancellationToken.None);
//                 await _emailStore.SetEmailAsync(user, Register.Email, CancellationToken.None);

//                 var result = await _userManager.CreateAsync(user, Register.Password);

//                 if (result.Succeeded)
//                 {
//                     _logger.LogInformation("User created a new account with password.");

//                     var userId = await _userManager.GetUserIdAsync(user);
//                     var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
//                     code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
//                     var callbackUrl = Url.Page(
//                         "/Account/ConfirmEmail",
//                         null,
//                         new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
//                         Request.Scheme);

//                     await _emailSender.SendEmailAsync(Register.Email, "Confirm your email",
//                         $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

//                     if (_userManager.Options.SignIn.RequireConfirmedAccount)
//                     {
//                         return RedirectToPage("RegisterConfirmation", new { email = Register.Email, returnUrl });
//                     }
//                     else
//                     {
//                         await _signInManager.SignInAsync(user, false);
//                         return LocalRedirect(returnUrl);
//                     }
//                 }

//                 foreach (var error in result.Errors)
//                     ModelState.AddModelError(string.Empty, error.Description);
//             }

//             return Page();
//         }

//         private IUserEmailStore<ApplicationUser> GetEmailStore()
//         {
//             if (!_userManager.SupportsUserEmail)
//                 throw new NotSupportedException("User store must support email.");

//             return (IUserEmailStore<ApplicationUser>)_userStore;
//         }
//     }
// }


public class AuthModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;
    private readonly ILogger<AuthModel> _logger;
    private readonly IEmailSender _emailSender;

    public AuthModel(
        UserManager<ApplicationUser> userManager,
        IUserStore<ApplicationUser> userStore,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AuthModel> logger,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _signInManager = signInManager;
        _logger = logger;
        _emailSender = emailSender;
        ExternalLogins = new List<AuthenticationScheme>();
    }

    public IList<AuthenticationScheme> ExternalLogins { get; set; }
    public string ReturnUrl { get; set; }

    public class LoginInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class RegisterInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }

    public async Task OnGetAsync(string returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostLoginAsync(LoginInputModel login, string returnUrl = null)
    {
        Console.WriteLine("THIS IS LOGIN");
        Console.WriteLine("EMAIL: ", login.Email);
        Console.WriteLine("PASSWORD: ", login.Password);
        foreach (var state in ModelState)
        {
            Console.WriteLine($"{state.Key}: {state.Value.Errors.Count} errors");
            foreach (var error in state.Value.Errors)
                Console.WriteLine($"   {error.ErrorMessage}");
        }

        returnUrl ??= Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (!ModelState.IsValid) return Page();

        var result = await _signInManager.PasswordSignInAsync(
            login.Email, login.Password, login.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
            return LocalRedirect(returnUrl);

        ModelState.AddModelError(string.Empty, "Невдала спроба входу.");
        return Page();
    }

    public async Task<IActionResult> OnPostRegisterAsync(RegisterInputModel register, string returnUrl = null)
    {
        Console.WriteLine("EMAIL: ", register.Email);
        Console.WriteLine("PASSWORD: ", register.Password);
        Console.WriteLine("CONFIRM PASSWORD: ", register.ConfirmPassword);

        foreach (var state in ModelState)
        {
            Console.WriteLine($"{state.Key}: {state.Value.Errors.Count} errors");
            foreach (var error in state.Value.Errors)
                Console.WriteLine($"   {error.ErrorMessage}");
        }

        returnUrl ??= Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (!ModelState.IsValid) return Page();


        var user = new ApplicationUser();
        await _userStore.SetUserNameAsync(user, register.Email, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, register.Email, CancellationToken.None);

        var result = await _userManager.CreateAsync(user, register.Password);
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(returnUrl);
        }


        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return Page();
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
            throw new NotSupportedException("User store must support email.");

        return (IUserEmailStore<ApplicationUser>)_userStore;
    }
}
