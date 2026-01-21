using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjAvaliacao.Data;
using ProjAvaliacao.Models;
using ProjAvaliacao.ViewModels;

namespace ProjAvaliacao.Controllers
{
    [Authorize(Roles = "Aluno")]
    public class AvaliacaoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public AvaliacaoController(
            ApplicationDbContext context,
            UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<Aluno?> ObterAlunoLogadoAsync()
        {
            var usuario = await _userManager.GetUserAsync(User);

            return await _context.Alunos
                .FirstOrDefaultAsync(a => a.UsuarioId == usuario.Id);
        }

        [HttpGet]
        public async Task<IActionResult> Criar(int matriculaId)
        {
            var aluno = await ObterAlunoLogadoAsync();
            if (aluno == null) return Unauthorized();

            var matricula = await _context.Matriculas
                .Include(m => m.DisciplinaOfertada)
                    .ThenInclude(d => d.Disciplina)
                .Include(m => m.DisciplinaOfertada)
                    .ThenInclude(d => d.Professor)
                .FirstOrDefaultAsync(m => m.Id == matriculaId && m.AlunoId.ToString() == aluno.Id.ToString());

            if (matricula == null)
                return NotFound();

            if (!matricula.DisciplinaOfertada.Ativa)
                return BadRequest("Disciplina não está ativa para avaliação.");

            var viewModel = new AvaliacaoCreateViewModel
            {
                MatriculaId = matricula.Id,
                Disciplina = matricula.DisciplinaOfertada.Disciplina.Nome,
                Professor = matricula.DisciplinaOfertada.Professor.Nome,
                Periodo = $"{matricula.DisciplinaOfertada.Ano}/{matricula.DisciplinaOfertada.Semestre}"
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(AvaliacaoCreateViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var aluno = await ObterAlunoLogadoAsync();
            if (aluno == null) return Unauthorized();

            var matricula = await _context.Matriculas
                .FirstOrDefaultAsync(m => m.Id == vm.MatriculaId && m.AlunoId.ToString() == aluno.Id.ToString());

            if (matricula == null)
                return NotFound();

            var JaAvaliada = await _context.Avaliacoes
                .AnyAsync(a => a.MatriculaId == vm.MatriculaId);

            if (JaAvaliada) 
                return BadRequest("Esta matrícula já foi avaliada.");

            var avaliacao = new Avaliacao
            {
                MatriculaId = vm.MatriculaId,
                Nota = vm.Nota,
                Comentario = vm.Comentario,
                Recomendaria = vm.Recomendaria,
                DataAvaliacao = DateTime.UtcNow
            };

            _context.Avaliacoes.Add(avaliacao);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var aluno = await ObterAlunoLogadoAsync();
            if (aluno == null) return Unauthorized();

            var matriculasAvaliaveis = await _context.Matriculas
                .Include(m => m.DisciplinaOfertada)
                .ThenInclude(d => d.Disciplina)
                .Include(m => m.DisciplinaOfertada)
                .ThenInclude(d => d.Professor)
                .Where(m => m.AlunoId.ToString() == aluno.Id.ToString() && m.DisciplinaOfertada.Ativa &&
                    !_context.Avaliacoes.Any(a => a.MatriculaId == m.Id))
                .ToListAsync();

            return View(matriculasAvaliaveis);
        }

        [HttpGet]
        public async Task<IActionResult> Historico()
        {
            var aluno = await ObterAlunoLogadoAsync();
            if (aluno == null) return Unauthorized();
            
            var avaliacoes = await _context.Avaliacoes
                .Include(a => a.Matricula)
                    .ThenInclude(m => m.DisciplinaOfertada)
                        .ThenInclude(d => d.Disciplina)
                .Include(a => a.Matricula)
                    .ThenInclude(m => m.DisciplinaOfertada)
                        .ThenInclude(d => d.Professor)
                .Where(a => a.Matricula.AlunoId.ToString() == aluno.Id.ToString())
                .OrderByDescending(a => a.DataAvaliacao)
                .ToListAsync();

            return View(avaliacoes);
        }
    }
}