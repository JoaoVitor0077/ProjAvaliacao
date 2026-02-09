using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjAvaliacao.Data;
using ProjAvaliacao.Models;
using ProjAvaliacao.ViewModels;

namespace ProjAvaliacao.Controllers
{
    // Restringe o acesso a usuários no papel "Aluno"
    [Authorize(Roles = "Aluno")]
    public class AvaliacaoController : Controller
    {
        // Dependências injetadas: contexto do EF e gerenciador de usuários
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        // Construtor: recebe e armazena as dependências
        public AvaliacaoController(
            ApplicationDbContext context,
            UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Método auxiliar privado que retorna o registro de Aluno
        // correspondente ao usuário logado (ou null se não existir)
        private async Task<Aluno?> ObterAlunoLogadoAsync()
        {
            var usuario = await _userManager.GetUserAsync(User);

            return await _context.Alunos
                .FirstOrDefaultAsync(a => a.UsuarioId == usuario.Id);
        }

        // Ação GET: exibe o formulário de criação de avaliação para uma matrícula
        [HttpGet]
        public async Task<IActionResult> Criar(int matriculaId)
        {
            var aluno = await ObterAlunoLogadoAsync();
            if (aluno == null) return Unauthorized();

            // Carrega a matrícula com dados da disciplina e professor
            var matricula = await _context.Matriculas
                .Include(m => m.DisciplinaOfertada)
                    .ThenInclude(d => d.Disciplina)
                .Include(m => m.DisciplinaOfertada)
                    .ThenInclude(d => d.Professor)
                .FirstOrDefaultAsync(m => m.Id == matriculaId && m.AlunoId.ToString() == aluno.Id.ToString());

            if (matricula == null)
                return NotFound();

            // Verifica se a disciplina ofertada está ativa para avaliação
            if (!matricula.DisciplinaOfertada.Ativa)
                return BadRequest("Disciplina não está ativa para avaliação.");

            // Prepara o ViewModel com informações a serem exibidas no formulário
            var viewModel = new AvaliacaoCreateViewModel
            {
                MatriculaId = matricula.Id,
                Disciplina = matricula.DisciplinaOfertada.Disciplina.Nome,
                Professor = matricula.DisciplinaOfertada.Professor.Nome,
                Periodo = $"{matricula.DisciplinaOfertada.Ano}/{matricula.DisciplinaOfertada.Semestre}"
            };

            return View(viewModel);
        }

        // Ação POST: processa o envio do formulário de avaliação
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(AvaliacaoCreateViewModel vm)
        {
            // Validação do modelo; em caso de erro, reexibe o formulário
            if (!ModelState.IsValid)
            {
                // Coleta erros para depuração e coloca em TempData (JSON) para inspeção rápida
                var errors = ModelState
                    .Where(m => m.Value.Errors.Count > 0)
                    .Select(kvp => new { Field = kvp.Key, Errors = kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray() })
                    .ToList();

                TempData["ModelErrors"] = System.Text.Json.JsonSerializer.Serialize(errors);

                return View(vm);
            }

            var aluno = await ObterAlunoLogadoAsync();
            if (aluno == null) return Unauthorized();

            // Garante que a matrícula pertence ao aluno logado
            var matricula = await _context.Matriculas
                .FirstOrDefaultAsync(m => m.Id == vm.MatriculaId && m.AlunoId.ToString() == aluno.Id.ToString());

            if (matricula == null)
                return NotFound();

            // Verifica se já existe uma avaliação para essa matrícula
            var JaAvaliada = await _context.Avaliacoes
                .AnyAsync(a => a.MatriculaId == vm.MatriculaId);

            if (JaAvaliada) 
                return BadRequest("Esta matrícula já foi avaliada.");

            // Cria a entidade Avaliacao e salva no banco
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

            // Redireciona para a página inicial após sucesso
            return RedirectToAction("Index", "Home");
        }

        // Ação GET: lista as matrículas que o aluno ainda pode avaliar
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var aluno = await ObterAlunoLogadoAsync();
            if (aluno == null) return Unauthorized();

            // Busca matrículas ativas sem avaliação já existente
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

        // Ação GET: exibe o histórico de avaliações do aluno (ordenado por data)
        [HttpGet]
        public async Task<IActionResult> Historico()
        {
            var aluno = await ObterAlunoLogadoAsync();
            if (aluno == null) return Unauthorized();
            
            // Carrega avaliações com dados relacionados de matrícula, disciplina e professor
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