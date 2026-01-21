using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ProjAvaliacao.Models;

namespace ProjAvaliacao.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<Usuario> signInManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Identificador obrigatório")]
            [Display(Name = "Usuário ou E-mail")]
            public string Identificador { get; set; }

            [Required(ErrorMessage = "Senha obrigatória")]
            [DataType(DataType.Password)]
            [Display(Name = "Senha")]
            public string Senha { get; set; }

            [Display(Name = "Lembrar de mim")]
            public bool LembrarMe { get; set; }
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(Input.Identificador, Input.Senha, Input.LembrarMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("Usuário logado.");
                return LocalRedirect(returnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.LembrarMe });
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("Conta bloqueada.");
                return RedirectToPage("./Lockout");
            }

            ModelState.AddModelError(string.Empty, "Tentativa inválida de login.");
            return Page();
        }
    }
}