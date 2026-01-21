using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjAvaliacao.Data;
using ProjAvaliacao.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

public static class IdentitySeed
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // 1️⃣ Criar Roles
        string[] roles = { "Aluno", "Professor", "Administrador" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2️⃣ Criar Admin
        await CriarAdministrador(userManager);

        // 3️⃣ Criar Professor
        await CriarProfessor(userManager, context);

        // 4️⃣ Criar Aluno
        await CriarAluno(userManager, context);

        // 5️⃣ Criar Disciplinas
        await CriarDisciplinas(context);

        // 6️⃣ Criar Disciplinas ofertadas (liga disciplina + professor)
        await CriarDisciplinasOfertadas(context);

        // 7️⃣ Criar Matrículas
        await CriarMatriculas(context);

        // 8️⃣ Criar Avaliações
        await CriarAvaliacoes(context);
    }

    // -----------------------------

    private static async Task CriarAdministrador(UserManager<Usuario> userManager)
    {
        const string email = "admin@institucional.com";

        if (await userManager.FindByEmailAsync(email) != null)
            return;

        var admin = new Usuario
        {
            UserName = email,
            Email = email,
            Nome = "Administrador",
            Sobrenome = "Sistema",
            EmailConfirmed = true
        };

        await userManager.CreateAsync(admin, "Admin@123");
        await userManager.AddToRoleAsync(admin, "Administrador");
    }

    // -----------------------------

    private static async Task CriarProfessor(
        UserManager<Usuario> userManager,
        ApplicationDbContext context)
    {
        const string email = "professor@institucional.com";

        if (await userManager.FindByEmailAsync(email) != null)
            return;

        var usuario = new Usuario
        {
            UserName = email,
            Email = email,
            Nome = "Carlos",
            Sobrenome = "Silva",
            EmailConfirmed = true
        };

        await userManager.CreateAsync(usuario, "Professor@123");
        await userManager.AddToRoleAsync(usuario, "Professor");

        context.Professores.Add(new Professor
        {
            Nome = $"{usuario.Nome} {usuario.Sobrenome}",
            Email = usuario.Email,
            UsuarioId = usuario.Id
        });

        await context.SaveChangesAsync();
    }

    // -----------------------------

    private static async Task CriarAluno(
        UserManager<Usuario> userManager,
        ApplicationDbContext context)
    {
        const string email = "aluno@institucional.com";

        if (await userManager.FindByEmailAsync(email) != null)
            return;

        var usuario = new Usuario
        {
            UserName = email,
            Email = email,
            Nome = "João",
            Sobrenome = "Vitor",
            EmailConfirmed = true
        };

        await userManager.CreateAsync(usuario, "Aluno@123");
        await userManager.AddToRoleAsync(usuario, "Aluno");

        var aluno = new Aluno
        {
            UsuarioId = usuario.Id,
            RegistroAcademico = "RA20240001"
        };

        context.Alunos.Add(aluno);
        await context.SaveChangesAsync();
    }

    // -----------------------------
    // NOVAS FUNÇÕES DE SEED PARA AS OUTRAS TABELAS
    // -----------------------------

    private static async Task CriarDisciplinas(ApplicationDbContext context)
    {
        if (context.Disciplinas.Any())
            return;

        var disciplinas = new[]
        {
            new Disciplina { Nome = "Programação I", Codigo = "PROG101" },
            new Disciplina { Nome = "Algoritmos e Estruturas de Dados", Codigo = "AED201" },
            new Disciplina { Nome = "Banco de Dados", Codigo = "BD301" }
        };

        context.Disciplinas.AddRange(disciplinas);
        await context.SaveChangesAsync();
    }

    private static async Task CriarDisciplinasOfertadas(ApplicationDbContext context)
    {
        if (context.DisciplinaOfertadas.Any())
            return;

        // Pegar uma disciplina e um professor existentes
        var disciplina = await context.Disciplinas.FirstOrDefaultAsync();
        var professor = await context.Professores.FirstOrDefaultAsync();

        if (disciplina == null || professor == null)
            return; // exigimos que existam disciplina e professor

        var ano = DateTime.UtcNow.Year;
        var ofertas = new[]
        {
            new DisciplinaOfertada
            {
                DisciplinaId = disciplina.Id,
                ProfessorId = professor.Id,
                Ano = ano,
                Semestre = 1,
                Ativa = true
            },
            // criar outra oferta usando próxima disciplina (se existir)
            new DisciplinaOfertada
            {
                DisciplinaId = (await context.Disciplinas.Skip(1).FirstOrDefaultAsync())?.Id ?? disciplina.Id,
                ProfessorId = professor.Id,
                Ano = ano,
                Semestre = 2,
                Ativa = true
            }
        };

        context.DisciplinaOfertadas.AddRange(ofertas);
        await context.SaveChangesAsync();
    }

    private static async Task CriarMatriculas(ApplicationDbContext context)
    {
        if (context.Matriculas.Any())
            return;

        var aluno = await context.Alunos.FirstOrDefaultAsync();
        var oferta = await context.DisciplinaOfertadas.FirstOrDefaultAsync();

        if (aluno == null || oferta == null)
            return;

        var matricula = new Matricula
        {
            AlunoId = aluno.Id,
            DisciplinaOfertadaId = oferta.Id,
            Status = StatusMatricula.Ativa,
            DataMatricula = DateTime.UtcNow
        };

        context.Matriculas.Add(matricula);
        await context.SaveChangesAsync();
    }

    private static async Task CriarAvaliacoes(ApplicationDbContext context)
    {
        if (context.Avaliacoes.Any())
            return;

        // Garantir que exista matrícula
        var matricula = await context.Matriculas.FirstOrDefaultAsync();

        if (matricula == null)
            return;

        var avaliacao = new Avaliacao
        {
            MatriculaId = matricula.Id,
            Nota = 9,
            Comentario = "Ótima disciplina e didática do professor.",
            Recomendaria = true,
            DataAvaliacao = DateTime.UtcNow
        };

        context.Avaliacoes.Add(avaliacao);
        await context.SaveChangesAsync();
    }
}
