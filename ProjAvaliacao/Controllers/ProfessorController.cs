using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjAvaliacao.Data;
using ProjAvaliacao.Models;
using ProjAvaliacao.ViewModels;

[Authorize(Roles = "Professor")]
public class ProfessorController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Usuario> _userManager;

    public ProfessorController(
        ApplicationDbContext context,
        UserManager<Usuario> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Dashboard()
    {
        // Obtém o usuário atualmente autenticado
        var usuario = await _userManager.GetUserAsync(User);
        // Consulta as disciplinas ofertadas pelo professor autenticado
        var dados = await _context.DisciplinaOfertadas
            // Validação para garantir que o professor só veja suas próprias disciplinas
            .Where(d => d.Professor.UsuarioId == usuario.Id)
            // Inclui as avaliações relacionadas
            .Select(d => new ProfessorDashboardViewModel
            {
                Disciplina = d.Disciplina.Nome,
                Ano = d.Ano,
                Semestre = d.Semestre,

            })
            .ToListAsync();

        return View(dados);
    }
}