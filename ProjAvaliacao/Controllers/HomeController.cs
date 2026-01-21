using ProjAvaliacao.Data;
using ProjAvaliacao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ProjAvaliacao.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public HomeController(ApplicationDbContext context, UserManager<Usuario> userManager)
            : base(context, userManager)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Sobre()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}