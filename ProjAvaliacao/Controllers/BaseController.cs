using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjAvaliacao.Data;
using ProjAvaliacao.Models;

namespace ProjAvaliacao.Controllers
{
    // Controlador base personalizado
    public abstract class BaseController : Controller
    {
        protected readonly ApplicationDbContext _context;
        protected readonly UserManager<Usuario> _userManager;

        protected BaseController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Propriedade para obter o usuário logado
        protected async Task<Usuario?> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }
    }
}